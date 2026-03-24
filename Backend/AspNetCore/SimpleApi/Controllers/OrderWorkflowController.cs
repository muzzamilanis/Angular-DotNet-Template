using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;
using SimpleApi.Data;
using SimpleApi.Hubs;
using SimpleApi.Models;
using SimpleApi.Services;
using System.Text.Json;

namespace SimpleApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class OrderWorkflowController : BaseController
	{
		private readonly ApplicationDbContext _context;
		private readonly IHubContext<OrderWorkflowHub> _hubContext;
		private readonly ChatClient _chatClient;
		private readonly ICountryService _countryService;

		public OrderWorkflowController(
			ApplicationDbContext context,
			IHubContext<OrderWorkflowHub> hubContext,
			OpenAIClient openAIClient,
			IConfiguration configuration,
			ICountryService countryService)
		{
			_context = context;
			_hubContext = hubContext;
			var model = configuration["OpenAI:Model"] ?? "gpt-4";
			_chatClient = openAIClient.GetChatClient(model);
			_countryService = countryService;
		}

		/// <summary>GET /api/orderworkflow/{orderId} — full status history for the workflow diagram</summary>
		[HttpGet("{orderId}")]
		public async Task<IActionResult> GetWorkflow(int orderId)
		{
			try
			{
				var statuses = await _context.OrderWorkflowStatuses
					.Where(s => s.OrderId == orderId)
					.OrderBy(s => s.Timestamp)
					.Select(s => new
					{
						s.Id,
						s.OrderId,
						s.Stage,
						s.AssignedTo,
						s.UpdatedBy,
						s.Timestamp,
						s.Notes
					})
					.ToListAsync();

				return Ok(statuses);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>POST /api/orderworkflow/update-status — called by agents/system to advance status stages</summary>
		[HttpPost("update-status")]
		[Authorize]  // Can tighten to [Authorize(Roles = "System,SuperAdmin")] when roles are configured
		public async Task<IActionResult> UpdateStatus([FromBody] UpdateWorkflowStatusRequest req)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(req.Stage))
					return BadRequest("Stage is required.");

				var entry = new OrderWorkflowStatus
				{
					OrderId = req.OrderId,
					Stage = req.Stage,
					UpdatedBy = req.UpdatedBy ?? "System",
					AssignedTo = req.AssignedTo,
					Notes = req.Notes,
					Timestamp = DateTime.UtcNow
				};

				_context.OrderWorkflowStatuses.Add(entry);
				await _context.SaveChangesAsync();

				// Notify all SignalR clients subscribed to this order
				await _hubContext.Clients.Group($"order_{req.OrderId}")
					.SendAsync("WorkflowUpdated", new
					{
						stage = req.Stage,
						assignedTo = req.AssignedTo,
						updatedBy = req.UpdatedBy ?? "System",
						notes = req.Notes,
						timestamp = entry.Timestamp
					});

				return Ok(new { message = "Status updated.", entryId = entry.Id });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>POST /api/orderworkflow/parse-order — accepts natural language, returns a structured order preview via GPT-4</summary>
		[HttpPost("parse-order")]
		public async Task<IActionResult> ParseOrder([FromBody] ParseOrderRequest req)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(req.Text))
					return BadRequest("Order description text is required.");

				// Generate the next sequential SO number from the database
				var orderCount = await _context.SaleOrders.CountAsync();
				var nextOrderNumber = $"SO-{(orderCount + 1):D4}";

				// Fetch active service types for this company so AI can match them
				int companyId;
				try { companyId = GetCurrentUserCompanyId(); } catch { companyId = 0; }

				var serviceTypes = await _context.ServiceTypes
					.Where(st => st.IsActive && (companyId == 0 || st.CompanyId == companyId))
					.Select(st => new { st.Id, st.Name })
					.ToListAsync();

				var serviceTypeList = serviceTypes.Any()
					? string.Join(", ", serviceTypes.Select(st => $"{st.Id}:{st.Name}"))
					: "none";

				var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
				var systemPrompt = $"""
					You are an order parsing assistant. Today's date is {today}.
					Available service types (id:name): {serviceTypeList}
					Extract structured order details from the given natural language text.

					IMPORTANT — lookup_country tool usage:
					Call the lookup_country tool ONLY when the text contains a clear geographic/currency context clue:
					  • Explicit country or region names (e.g. "UAE", "United States", "Pakistan")
					  • City names used as an origin or location (e.g. "from Dubai", "client in Karachi", "based in London")
					  • Currency names or symbols (e.g. "Dirhams", "Dollars", "£", "PKR")
					Do NOT call lookup_country for personal names, company names, or product names that happen to share a city/country name.
					  Example: "Order writing for Nagda" — "Nagda" here is a client/person name, NOT a location. Do not call lookup_country for it.
					  Example: "from Dubai for 1000 Dirhams" — call lookup_country("Dubai") because "from" signals a geographic origin.
					Use the tool result's "name" for the country field and "currency" for the currencyCode field.
					If the tool returns null, set country and currencyCode to null.

					Return ONLY a valid JSON object with these exact fields:
					- clientName (string or null): the client or customer name mentioned, or null if not found
					- deadline (string or null): resolve the deadline to an absolute date in "yyyy-MM-dd" format using today's date as the reference for relative expressions like "in 3 days", "next Monday", "by Friday". Return null only if no deadline is mentioned at all.
					- wordsOrPages (string or null): the exact word or page count if mentioned (e.g. "2000 Words", "5 Pages"). Return null if not mentioned.
					- orderDetails (string): a clean, concise summary of what is being ordered
					- country (string or null): use the "name" from the lookup_country tool result. Return null if no location mentioned or tool returned null.
					- currencyCode (string or null): use the "currency" from the lookup_country tool result, or infer ISO code from an explicit currency name (e.g. "AED" for Dirham/Dirhams). Return null if nothing found.
					- currencyAmount (number or null): the numeric monetary amount mentioned (e.g. 1000 from "1000 Dirhams"). Return null if not mentioned.
					- serviceTypeId (number or null): from the available service types list pick the ID that best matches the order. Return null if no match or list is "none".
					- serviceTypeName (string or null): the name corresponding to serviceTypeId. Return null if no match.
					- confidence (number): a value between 0 and 1 representing how confident you are in the extraction
					Do not include any explanation, markdown, or code fences — only the raw JSON object.
					""";

				// ── Tool: lookup_country ──────────────────────────────────────────────────
				var lookupCountryTool = ChatTool.CreateFunctionTool(
					functionName: "lookup_country",
					functionDescription: "Validates a country, city, or location mention against the supported countries list. Returns the official country name, ISO country code, currency code, and symbol — or null if not found.",
					functionParameters: BinaryData.FromString("""
						{
							"type": "object",
							"properties": {
								"hint": {
									"type": "string",
									"description": "The country name, city, or location mentioned by the user (e.g. 'Dubai', 'UAE', 'United Arab Emirates', 'Dirhams', 'America')"
								}
							},
							"required": ["hint"]
						}
						""")
				);

				var completionOptions = new ChatCompletionOptions();
				completionOptions.Tools.Add(lookupCountryTool);

				var messages = new List<ChatMessage>
				{
					new SystemChatMessage(systemPrompt),
					new UserChatMessage(req.Text),
				};

				// ── Agentic tool-call loop ────────────────────────────────────────────────
				var completion = await _chatClient.CompleteChatAsync(messages, completionOptions);
				var loopGuard = 0;

				while (completion.Value.FinishReason == ChatFinishReason.ToolCalls && loopGuard++ < 5)
				{
					messages.Add(new AssistantChatMessage(completion.Value));

					foreach (var toolCall in completion.Value.ToolCalls)
					{
						if (toolCall.FunctionName == "lookup_country")
						{
							using var argsDoc = JsonDocument.Parse(toolCall.FunctionArguments);
							var hint = argsDoc.RootElement.TryGetProperty("hint", out var hintEl)
								? hintEl.GetString() ?? ""
								: "";

							var entry = _countryService.Lookup(hint);
							var toolResult = entry is not null
								? JsonSerializer.Serialize(new { name = entry.Name, code = entry.Code, currency = entry.Currency, symbol = entry.Symbol })
								: "{\"error\":\"Country not found in supported list\"}";

							messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
						}
					}

					completion = await _chatClient.CompleteChatAsync(messages, completionOptions);
				}

				var rawJson = completion.Value.Content[0].Text.Trim();

				// Strip markdown code fences if the AI wraps the response
				if (rawJson.StartsWith("```"))
				{
					rawJson = rawJson.Substring(rawJson.IndexOf('\n') + 1);
					if (rawJson.EndsWith("```"))
						rawJson = rawJson[..rawJson.LastIndexOf("```")].Trim();
				}

				var parsed = JsonSerializer.Deserialize<JsonElement>(rawJson);

				int? parsedServiceTypeId = null;
				if (parsed.TryGetProperty("serviceTypeId", out var stId) && stId.ValueKind == JsonValueKind.Number)
					parsedServiceTypeId = stId.GetInt32();

				decimal? parsedCurrencyAmount = null;
				if (parsed.TryGetProperty("currencyAmount", out var ca) && ca.ValueKind == JsonValueKind.Number)
					parsedCurrencyAmount = ca.GetDecimal();

				return Ok(new
				{
					originalText = req.Text,
					suggestedOrderId = nextOrderNumber,
					suggestedOrderDetails = parsed.TryGetProperty("orderDetails", out var od) ? od.GetString() : req.Text,
					suggestedClientName = parsed.TryGetProperty("clientName", out var cn) ? cn.GetString() : null,
					suggestedDeadline = parsed.TryGetProperty("deadline", out var dl) ? dl.GetString() : null,
					suggestedWordCount = parsed.TryGetProperty("wordsOrPages", out var wc) ? wc.GetString() : null,
					suggestedCountry = parsed.TryGetProperty("country", out var co) ? co.GetString() : null,
					suggestedCurrencyCode = parsed.TryGetProperty("currencyCode", out var cc) ? cc.GetString() : null,
					suggestedCurrencyAmount = parsedCurrencyAmount,
					suggestedServiceTypeId = parsedServiceTypeId,
					suggestedServiceTypeName = parsed.TryGetProperty("serviceTypeName", out var stn) ? stn.GetString() : null,
					confidence = parsed.TryGetProperty("confidence", out var c) ? c.GetDouble() : 0.0,
					isParsedByAI = true,
					note = "Parsed using OpenAI GPT-4"
				});
			}
			catch (JsonException)
			{
				return StatusCode(502, "AI returned an unexpected response format. Please try again.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// ── Request DTOs ─────────────────────────────────────────────────────────────

		public class UpdateWorkflowStatusRequest
		{
			public int OrderId { get; set; }
			public string Stage { get; set; } = string.Empty;
			public string? UpdatedBy { get; set; }
			public string? AssignedTo { get; set; }
			public string? Notes { get; set; }
		}

		public class ParseOrderRequest
		{
			public string Text { get; set; } = string.Empty;
		}
	}
}
