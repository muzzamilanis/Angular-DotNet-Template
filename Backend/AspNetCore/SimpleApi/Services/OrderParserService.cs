using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OpenAI;
using OpenAI.Chat;
using SimpleApi.Data;
using System.Text.Json;

namespace SimpleApi.Services
{
	public record ParsedOrderResult(
		string SuggestedOrderId,
		string OriginalText,
		string? OrderDetails,
		string? ClientName,
		string? Deadline,
		string? WordCount,
		string? Country,
		string? CurrencyCode,
		decimal? CurrencyAmount,
		decimal? PkrAmount,
		decimal? ExchangeRate,
		int? ServiceTypeId,
		string? ServiceTypeName,
		int? ProjectId,
		string? ContactPhone,
		string? ContactEmail,
		decimal? AmountReceived,
		decimal? AmountReceivedInPkr,
		string? PaymentStatus,
		decimal? TotalPaymentAmount,
		string? SourceOfPayment,
		double Confidence
	);

	public interface IOrderParserService
	{
		Task<ParsedOrderResult> ParseOrderAsync(string text, int companyId);
	}

	public class OrderParserService : IOrderParserService
	{
		private const int MaxRetryAttempts = 3;
		private static readonly TimeSpan ServiceTypeCacheDuration = TimeSpan.FromMinutes(5);

		private readonly ApplicationDbContext _context;
		private readonly ChatClient _chatClient;
		private readonly IExchangeRateService _exchangeRateService;
		private readonly IMemoryCache _cache;
		private readonly ILogger<OrderParserService> _logger;

		public OrderParserService(
			ApplicationDbContext context,
			OpenAIClient openAIClient,
			IConfiguration configuration,
			IExchangeRateService exchangeRateService,
			IMemoryCache cache,
			ILogger<OrderParserService> logger)
		{
			_context = context;
			_chatClient = openAIClient.GetChatClient(configuration["OpenAI:Model"] ?? "gpt-4");
			_exchangeRateService = exchangeRateService;
			_cache = cache;
			_logger = logger;
		}

		public async Task<ParsedOrderResult> ParseOrderAsync(string text, int companyId)
		{
			// Use MAX(Id) to avoid COUNT race conditions — this is an approximate preview number
			var maxId = await _context.SaleOrders.MaxAsync(o => (int?)o.Id) ?? 0;
			var nextOrderNumber = $"SO-{(maxId + 1):D4}";

			var serviceTypeList = await GetServiceTypeListAsync(companyId);
			var projectList = await GetProjectListAsync(companyId);
			var systemPrompt = BuildSystemPrompt(DateTime.UtcNow.ToString("yyyy-MM-dd"), serviceTypeList, projectList);

			var messages = new List<ChatMessage>
			{
				new SystemChatMessage(systemPrompt),
				new UserChatMessage(text),
			};

			var completion = await CallWithRetryAsync(messages);
			var result = ParseCompletion(completion, nextOrderNumber, text);

			// Resolve PKR amount + exchange rate from live API
			if (result.CurrencyCode is not null && result.CurrencyAmount is not null)
			{
				try
				{
					var rateData = await _exchangeRateService.GetExchangeRateAsync(result.CurrencyCode);
					var pkr = Math.Round(result.CurrencyAmount.Value * rateData.ExchangeRate, 2);
					result = result with
					{
						PkrAmount = pkr,
						ExchangeRate = rateData.ExchangeRate,
						AmountReceivedInPkr = result.AmountReceived.HasValue
							? Math.Round(result.AmountReceived.Value * rateData.ExchangeRate, 2)
							: null
					};
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, "Could not fetch exchange rate for {CurrencyCode}", result.CurrencyCode);
				}
			}

			return result;
		}

		// ── Private helpers ───────────────────────────────────────────────────────────

		private async Task<string> GetServiceTypeListAsync(int companyId)
		{
			var cacheKey = $"service_types_{companyId}";
			if (_cache.TryGetValue(cacheKey, out string? cached) && cached is not null)
				return cached;

			var serviceTypes = await _context.ServiceTypes
				.Where(st => st.IsActive && (companyId == 0 || st.CompanyId == companyId))
				.Select(st => new { st.Id, st.Name })
				.ToListAsync();

			var list = serviceTypes.Any()
				? string.Join(", ", serviceTypes.Select(st => $"{st.Id}:{st.Name}"))
				: "none";

			_cache.Set(cacheKey, list, ServiceTypeCacheDuration);
			return list;
		}

		private async Task<string> GetProjectListAsync(int companyId)
		{
			var cacheKey = $"projects_{companyId}";
			if (_cache.TryGetValue(cacheKey, out string? cached) && cached is not null)
				return cached;

			var projects = await _context.Projects
				.Where(p => p.IsActive && (companyId == 0 || p.CompanyId == companyId))
				.Select(p => new { p.Id, p.ProjectName })
				.ToListAsync();

			var list = projects.Any()
				? string.Join(", ", projects.Select(p => $"{p.Id}:{p.ProjectName}"))
				: "none";

			_cache.Set(cacheKey, list, ServiceTypeCacheDuration);
			return list;
		}

		private static string BuildSystemPrompt(string today, string serviceTypeList, string projectList) => $"""
			You are an order parsing assistant. Today's date is {today}.
			Available service types (id:name): {serviceTypeList}
			Available projects (id:name): {projectList}
			Extract structured order details from the given natural language text.

			Return ONLY a valid JSON object with these exact fields:
			- clientName (string or null): the client or customer name mentioned, or null if not found
			- deadline (string or null): resolve the deadline to an absolute date in "yyyy-MM-dd" format using today's date. Return null only if no deadline is mentioned.
			- wordsOrPages (string or null): the exact word or page count if mentioned (e.g. "2000 Words", "5 Pages"). Return null if not mentioned.
			- orderDetails (string): a clean, concise summary of what is being ordered
			- country (string or null): the full country name if a country, city, or currency is mentioned (e.g. "Dubai" → "United Arab Emirates"). Return null if no location mentioned.
			- currencyCode (string or null): ISO 4217 code inferred from any location or currency mention (e.g. "Dubai" → "AED", "Dirhams" → "AED", "Pakistan" → "PKR"). Return null if nothing found.
			- currencyAmount (number or null): the total order price/amount mentioned (e.g. 1000 from "1000 Dirhams"). Return null if not mentioned.
			- amountReceived (number or null): any partial payment already received if explicitly stated (e.g. "paid 500", "received 200 advance", "payment 200 is received"). Return null if not mentioned.
			- paymentStatus (string): MUST be one of "Pending", "Partial", or "Paid". Use this logic strictly:
			    • "Paid" — if the full amount is stated as received/paid and no balance remains
			    • "Partial" — if a partial advance/payment is mentioned (amountReceived > 0 but less than total)
			    • "Pending" — if no payment at all is mentioned
			- totalPaymentAmount (number or null): same as currencyAmount unless a different total is explicitly stated. Return null if not mentioned.
			- sourceOfPayment (string or null): the payment channel or platform if mentioned (e.g. "Payoneer", "bank transfer", "PayPal", "cash", "Wise"). Return null if not mentioned — do NOT guess or default.
			- contactPhone (string or null): any phone number mentioned. Return null if not mentioned.
			- contactEmail (string or null): any email address mentioned. Return null if not mentioned.
			- serviceTypeId (number or null): from the available service types list pick the ID best matching the order type. Return null if no match or list is "none".
			- serviceTypeName (string or null): the name corresponding to serviceTypeId. Return null if no match.
			- projectId (number or null): from the available projects list pick the ID best matching any project name or topic mentioned. Return null if no match or list is "none".
			- confidence (number): a value between 0 and 1 representing how confident you are in the extraction
			Do not include any explanation, markdown, or code fences — only the raw JSON object.
			""";



		private async Task<ChatCompletion> CallWithRetryAsync(List<ChatMessage> messages)
		{
			var attempt = 0;
			while (true)
			{
				try
				{
					return (await _chatClient.CompleteChatAsync(messages)).Value;
				}
				catch (Exception ex) when (attempt < MaxRetryAttempts - 1)
				{
					attempt++;
					var delaySecs = Math.Pow(2, attempt);
					_logger.LogWarning(ex, "OpenAI call failed (attempt {Attempt}/{Max}), retrying in {Delay}s",
						attempt, MaxRetryAttempts, delaySecs);
					await Task.Delay(TimeSpan.FromSeconds(delaySecs));
				}
			}
		}

		private static ParsedOrderResult ParseCompletion(ChatCompletion completion, string nextOrderNumber, string originalText)
		{
			var rawJson = completion.Content[0].Text.Trim();

			if (rawJson.StartsWith("```"))
			{
				rawJson = rawJson[(rawJson.IndexOf('\n') + 1)..];
				if (rawJson.EndsWith("```"))
					rawJson = rawJson[..rawJson.LastIndexOf("```")].Trim();
			}

			var parsed = JsonSerializer.Deserialize<JsonElement>(rawJson);

			static int? GetNullableInt(JsonElement el, string key) =>
				el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetInt32() : null;

			static decimal? GetNullableDecimal(JsonElement el, string key) =>
				el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDecimal() : null;

			static string? GetNullableString(JsonElement el, string key) =>
				el.TryGetProperty(key, out var v) && v.ValueKind != JsonValueKind.Null ? v.GetString() : null;

			return new ParsedOrderResult(
				SuggestedOrderId: nextOrderNumber,
				OriginalText: originalText,
				OrderDetails: GetNullableString(parsed, "orderDetails") ?? originalText,
				ClientName: GetNullableString(parsed, "clientName"),
				Deadline: GetNullableString(parsed, "deadline"),
				WordCount: GetNullableString(parsed, "wordsOrPages"),
				Country: GetNullableString(parsed, "country"),
				CurrencyCode: GetNullableString(parsed, "currencyCode"),
				CurrencyAmount: GetNullableDecimal(parsed, "currencyAmount"),
				PkrAmount: null,       // populated by caller after exchange rate lookup
				ExchangeRate: null,    // populated by caller after exchange rate lookup
				ServiceTypeId: GetNullableInt(parsed, "serviceTypeId"),
				ServiceTypeName: GetNullableString(parsed, "serviceTypeName"),
				ProjectId: GetNullableInt(parsed, "projectId"),
				ContactPhone: GetNullableString(parsed, "contactPhone"),
				ContactEmail: GetNullableString(parsed, "contactEmail"),
				AmountReceived: GetNullableDecimal(parsed, "amountReceived"),
				AmountReceivedInPkr: null, // populated by caller after exchange rate lookup
				PaymentStatus: GetNullableString(parsed, "paymentStatus") ?? "Pending",
				TotalPaymentAmount: GetNullableDecimal(parsed, "totalPaymentAmount"),
				SourceOfPayment: GetNullableString(parsed, "sourceOfPayment"),
				Confidence: parsed.TryGetProperty("confidence", out var c) && c.ValueKind == JsonValueKind.Number ? c.GetDouble() : 0.0
			);
		}

		#region ARCHIVED — Tool Calling (replaced by simple prompt; no longer needed)
		/*
		private const int MaxToolCallIterations = 5;

		// _countryService field (only used in the tool call loop):
		// private readonly ICountryService _countryService;
		// ... injected via constructor: ICountryService countryService
		// ... assigned: _countryService = countryService;

		// Tool call setup + agentic loop that was inside ParseOrderAsync:
		//
		//   var options = new ChatCompletionOptions();
		//   options.Tools.Add(BuildLookupCountryTool());
		//
		//   var completion = await CallWithRetryAsync(messages, options);
		//   var loopGuard = 0;
		//
		//   while (completion.FinishReason == ChatFinishReason.ToolCalls && loopGuard++ < MaxToolCallIterations)
		//   {
		//       messages.Add(new AssistantChatMessage(completion));
		//
		//       foreach (var toolCall in completion.ToolCalls)
		//       {
		//           if (toolCall.FunctionName == "lookup_country")
		//           {
		//               using var argsDoc = JsonDocument.Parse(toolCall.FunctionArguments);
		//               var hint = argsDoc.RootElement.TryGetProperty("hint", out var hintEl)
		//                   ? hintEl.GetString() ?? ""
		//                   : "";
		//
		//               var entry = _countryService.Lookup(hint);
		//               var toolResult = entry is not null
		//                   ? JsonSerializer.Serialize(new { name = entry.Name, code = entry.Code, currency = entry.Currency, symbol = entry.Symbol })
		//                   : "{\"error\":\"Country not found in supported list\"}";
		//
		//               messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
		//           }
		//       }
		//
		//       completion = await CallWithRetryAsync(messages, options);
		//   }

		private static ChatTool BuildLookupCountryTool() => ChatTool.CreateFunctionTool(
			functionName: "lookup_country",
			functionDescription: "Validates a country, city, or location mention against the supported countries list. Returns the official country name, ISO country code, currency code, and symbol — or null if not found.",
			functionParameters: BinaryData.FromString("""
				{
					\"type\": \"object\",
					\"properties\": {
						\"hint\": {
							\"type\": \"string\",
							\"description\": \"The country name, city, or location mentioned by the user (e.g. 'Dubai', 'UAE', 'United Arab Emirates', 'Dirhams', 'America')\"
						}
					},
					\"required\": [\"hint\"]
				}
				""")
		);

		// Original CallWithRetryAsync with ChatCompletionOptions (for tool calls):
		// private async Task<ChatCompletion> CallWithRetryAsync(List<ChatMessage> messages, ChatCompletionOptions options)
		// {
		//     ... same body but passes options: _chatClient.CompleteChatAsync(messages, options)
		// }
		*/
		#endregion
	}
}
