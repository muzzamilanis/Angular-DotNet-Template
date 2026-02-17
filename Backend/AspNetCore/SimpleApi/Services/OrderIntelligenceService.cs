using SimpleApi.DTOs;
using SimpleApi.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public interface IOrderIntelligenceService
{
	Task<bool> TriggerIntelligenceAnalysisAsync(string orderId);
	Task SendWebhook(SaleOrderDto saleOrder);
}

public class OrderIntelligenceService : IOrderIntelligenceService
{
	private readonly HttpClient _httpClient;
	private readonly IConfiguration _config;
	private readonly ILogger<OrderIntelligenceService> _logger;

	public OrderIntelligenceService(
		IHttpClientFactory httpClientFactory,
		IConfiguration config,
		ILogger<OrderIntelligenceService> logger)
	{
		_httpClient = httpClientFactory.CreateClient();
		_config = config;
		_logger = logger;
	}

	public async Task<bool> TriggerIntelligenceAnalysisAsync(string orderId)
	{
		try
		{
			var agentUrl = _config["PythonAgent:IntelligenceAgentUrl"];
			var enabled = _config.GetValue<bool>("PythonAgent:Enabled");

			if (!enabled)
			{
				_logger.LogInformation("Python Intelligence Agent is disabled");
				return false;
			}

			var payload = new { orderId = orderId };
			var content = new StringContent(
				JsonSerializer.Serialize(payload),
				Encoding.UTF8,
				"application/json"
			);

			_logger.LogInformation($"Triggering Intelligence Agent for order {orderId}");

			var response = await _httpClient.PostAsync(agentUrl, content);

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation($"Intelligence analysis triggered for {orderId}");
				return true;
			}
			else
			{
				_logger.LogWarning($"Intelligence agent returned {response.StatusCode}");
				return false;
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Failed to trigger intelligence analysis for {orderId}");
			return false;
		}
	}

	public async Task SendWebhook(SaleOrderDto order)
	{
		using var httpClient = new HttpClient();
		
		string webhookUrl;
		if (_config.GetValue<string>("ASPNETCORE_ENVIRONMENT") != "Production")
		{
			webhookUrl = "http://localhost:5000/webhook/order-created"; // Local development URL
		}
		else
		{
			webhookUrl = _config["Webhook:AgentUrl"];
		}
		
		var webhookSecret = _config["Webhook:Secret"];
		
		var payload = new
		{
			orderId = order.Id,
			customerName = order.CustomerName,
			customerEmail = order.ContactEmail,
			topic = order.ProjectName,
			pages = order.WordsOrPages,
			deadline = order.Deadline,
			specialRequirements = order.OrderDetails
		};
		
		var request = new HttpRequestMessage(HttpMethod.Post, webhookUrl)
		{
			Content = JsonContent.Create(payload)
		};
		
		request.Headers.Add("X-Webhook-Secret", webhookSecret);
		
		try
		{
			var response = await httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();
		}
		catch (Exception ex)
		{
			// Log error but don't fail order creation
			_logger.LogError(ex, "Failed to send webhook");
		}
	}
}