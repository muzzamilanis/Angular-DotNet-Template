using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
		private static readonly HashSet<string> AllowedStages = new(StringComparer.Ordinal)
		{
			"OrderReceived", "AIProcessing", "WriterAssigned", "InProgress", "UnderReview", "Delivered"
		};

		private readonly ApplicationDbContext _context;
		private readonly IHubContext<OrderWorkflowHub> _hubContext;
		private readonly IOrderParserService _orderParser;
		private readonly ILogger<OrderWorkflowController> _logger;

		public OrderWorkflowController(
			ApplicationDbContext context,
			IHubContext<OrderWorkflowHub> hubContext,
			IOrderParserService orderParser,
			ILogger<OrderWorkflowController> logger)
		{
			_context = context;
			_hubContext = hubContext;
			_orderParser = orderParser;
			_logger = logger;
		}

		/// <summary>GET /api/orderworkflow/{orderId} â€” full status history for the workflow diagram</summary>
		[HttpGet("{orderId}")]
		public async Task<IActionResult> GetWorkflow(int orderId)
		{
			try
			{
				int companyId;
				try { companyId = GetCurrentUserCompanyId(); } catch { return Forbid(); }

				// Ensure the order belongs to the caller's company (IDOR protection)
				var orderExists = await _context.SaleOrders
					.AnyAsync(o => o.Id == orderId && o.CompanyId == companyId);

				if (!orderExists)
					return NotFound();

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
				_logger.LogError(ex, "Failed to fetch workflow for order {OrderId}", orderId);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>POST /api/orderworkflow/update-status â€” advances the workflow stage for an order</summary>
		[HttpPost("update-status")]
		[Authorize(Roles = "SuperAdmin,Admin,System")]
		public async Task<IActionResult> UpdateStatus([FromBody] UpdateWorkflowStatusRequest req)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(req.Stage))
					return BadRequest("Stage is required.");

				if (!AllowedStages.Contains(req.Stage))
					return BadRequest($"Invalid stage '{req.Stage}'. Allowed: {string.Join(", ", AllowedStages)}");

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

				await _hubContext.Clients.Group($"order_{req.OrderId}")
					.SendAsync("WorkflowUpdated", new
					{
						stage = req.Stage,
						assignedTo = req.AssignedTo,
						updatedBy = req.UpdatedBy ?? "System",
						notes = req.Notes,
						timestamp = entry.Timestamp
					});

				_logger.LogInformation("Workflow stage for order {OrderId} advanced to {Stage}", req.OrderId, req.Stage);
				return Ok(new { message = "Status updated.", entryId = entry.Id });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to update workflow status for order {OrderId}", req.OrderId);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>POST /api/orderworkflow/parse-order â€” accepts natural language, returns a structured order preview via GPT-4</summary>
		[HttpPost("parse-order")]
		public async Task<IActionResult> ParseOrder([FromBody] ParseOrderRequest req)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(req.Text))
					return BadRequest("Order description text is required.");

				int companyId;
				try { companyId = GetCurrentUserCompanyId(); } catch { companyId = 0; }

				_logger.LogInformation("Parsing order for company {CompanyId}", companyId);

				var result = await _orderParser.ParseOrderAsync(req.Text, companyId);

				return Ok(new
				{
					originalText = result.OriginalText,
					suggestedOrderId = result.SuggestedOrderId,
					suggestedOrderDetails = result.OrderDetails,
					suggestedClientName = result.ClientName,
					suggestedDeadline = result.Deadline,
					suggestedWordCount = result.WordCount,
					suggestedCountry = result.Country,
					suggestedCurrencyCode = result.CurrencyCode,
					suggestedCurrencyAmount = result.CurrencyAmount,
					suggestedPkrCurrencyAmount = result.PkrAmount,
					suggestedExchangeRate = result.ExchangeRate,
					suggestedAmountRecieved = result.AmountReceived,
					suggestedAmountReceivedInPkr = result.AmountReceivedInPkr,
					suggestedServiceTypeId = result.ServiceTypeId,
					suggestedServiceTypeName = result.ServiceTypeName,
					suggestedProjectId = result.ProjectId,
					suggestedContactPhone = result.ContactPhone,
					suggestedContactEmail = result.ContactEmail,
					paymentStatus = result.PaymentStatus,
					totalPaymentAmount = result.TotalPaymentAmount,
					sourceOfPayment = result.SourceOfPayment,
					confidence = result.Confidence,
					isParsedByAI = true,
					note = "Parsed using OpenAI GPT-4"
				});
			}
			catch (JsonException)
			{
				_logger.LogWarning("AI returned unparseable JSON for input: {Text}", req.Text);
				return StatusCode(502, "AI returned an unexpected response format. Please try again.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to parse order text");
				return BadRequest(ex.Message);
			}
		}

		// â”€â”€ Request DTOs â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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
