using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;
using SimpleApi.Data;
using SimpleApi.Hubs;
using SimpleApi.Models;
using System.Text.Json;

namespace SimpleApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class OrderWorkflowController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IHubContext<OrderWorkflowHub> _hubContext;
		private readonly ChatClient _chatClient;

		public OrderWorkflowController(ApplicationDbContext context, IHubContext<OrderWorkflowHub> hubContext, OpenAIClient openAIClient, IConfiguration configuration)
		{
			_context = context;
			_hubContext = hubContext;
			var model = configuration["OpenAI:Model"] ?? "gpt-4";
			_chatClient = openAIClient.GetChatClient(model);
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

				const string systemPrompt = """
					You are an order parsing assistant. Extract structured order details from the given natural language text.
					Return ONLY a valid JSON object with these fields:
					- orderId (string): generate Serialize OrderId with SO-0001 or null if not found clientName.
					- clientName (string): the client or customer name mentioned, or null if not found
					- deadline (string): the deadline as "yyyy-MM-dd" if mentioned, otherwise null
					- wordCount (string): word/page count if mentioned (e.g. "500 words"), otherwise "Not specified"
					- orderDetails (string): a clean, concise summary of what is being ordered
					- confidence (number): a value between 0 and 1 representing how confident you are in the extraction
					Do not include any explanation, markdown, or code fences — only the raw JSON object.
					""";

				var messages = new List<ChatMessage>
				{
					new SystemChatMessage(systemPrompt),
					new UserChatMessage(req.Text)
				};

				var completion = await _chatClient.CompleteChatAsync(messages);
				var rawJson = completion.Value.Content[0].Text;

				var parsed = JsonSerializer.Deserialize<JsonElement>(rawJson);

				return Ok(new
				{
					originalText = req.Text,
					suggestedOrderId = parsed.TryGetProperty("orderId", out var oid) ? oid.GetString() : null,
					suggestedOrderDetails = parsed.TryGetProperty("orderDetails", out var od) ? od.GetString() : req.Text,
					suggestedClientName = parsed.TryGetProperty("clientName", out var cn) ? cn.GetString() : "Unknown Client",
					suggestedDeadline = parsed.TryGetProperty("deadline", out var dl) ? dl.GetString() : null,
					suggestedWordCount = parsed.TryGetProperty("wordCount", out var wc) ? wc.GetString() : "Not specified",
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
