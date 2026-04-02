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

		/// <summary>POST /api/orderworkflow/update-order — updates a "New" order via natural language chat. Locked once status changes.</summary>
		[HttpPost("update-order")]
		public async Task<IActionResult> UpdateOrderViaChat([FromBody] UpdateOrderViaChatRequest req)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(req.Text))
					return BadRequest("Update description text is required.");

				int companyId;
				try { companyId = GetCurrentUserCompanyId(); } catch { return Forbid(); }

				// Load existing order and enforce "New" status lock
				var saleOrder = await _context.SaleOrders
					.Include(o => o.Items)
					.FirstOrDefaultAsync(o => o.Id == req.OrderId && o.CompanyId == companyId);

				if (saleOrder == null)
					return NotFound("Order not found.");

				if (saleOrder.Status != "New")
					return Conflict("This order can no longer be modified. Orders are locked once their status progresses beyond 'New'.");

				// Parse the natural language update request via AI
				var parsed = await _orderParser.ParseOrderAsync(req.Text, companyId);

				// Merge AI results onto the existing order — only override fields the AI actually found
				if (parsed.ClientName != null)      { saleOrder.ClientName = parsed.ClientName; saleOrder.CustomerName = parsed.ClientName; }
				if (parsed.OrderDetails != null)    saleOrder.OrderDetails = parsed.OrderDetails;
				if (parsed.Deadline != null)        saleOrder.Deadline = DateTime.TryParse(parsed.Deadline, out var dl) ? dl : saleOrder.Deadline;
				if (parsed.WordCount != null)       saleOrder.WordsOrPages = parsed.WordCount;
				if (parsed.Country != null)         saleOrder.Country = parsed.Country;
				if (parsed.CurrencyCode != null)    saleOrder.CurrencyCode = parsed.CurrencyCode;
				if (parsed.CurrencyAmount != null)  saleOrder.ForeignCurrencyAmount = parsed.CurrencyAmount;
				if (parsed.PkrAmount != null)       saleOrder.PkrAmount = parsed.PkrAmount;
				if (parsed.ExchangeRate != null)    saleOrder.ExchangeRateUsed = parsed.ExchangeRate;
				if (parsed.ServiceTypeId != null)   saleOrder.ServiceTypeId = parsed.ServiceTypeId;
				if (parsed.ProjectId != null)       saleOrder.ProjectId = parsed.ProjectId;
				if (parsed.ContactPhone != null)    saleOrder.ContactPhone = parsed.ContactPhone;
				if (parsed.ContactEmail != null)    saleOrder.ContactEmail = parsed.ContactEmail;
				if (parsed.AmountReceived != null)  saleOrder.AmountReceived = parsed.AmountReceived;
				if (parsed.AmountReceivedInPkr != null) saleOrder.AmountReceivedInPkr = parsed.AmountReceivedInPkr;
				if (parsed.PaymentStatus != null)   saleOrder.PaymentStatus = parsed.PaymentStatus;
				if (parsed.TotalPaymentAmount != null) saleOrder.TotalPayment = parsed.TotalPaymentAmount;
				if (parsed.SourceOfPayment != null) saleOrder.SourceOfPayment = parsed.SourceOfPayment;

				// Persist the CSR chat input as an OrderInstruction for full audit trail
				int csrUserId;
				try { csrUserId = GetCurrentUserId(); } catch { csrUserId = 0; }
				if (csrUserId > 0)
				{
					_context.OrderInstructions.Add(new OrderInstruction
					{
						SaleOrderId = saleOrder.Id,
						SenderId = csrUserId,
						Message = $"[CSR Chat Update] {req.Text.Trim()}",
						CreatedAt = DateTime.UtcNow,
						IsFromTeamLead = false
					});
				}

				await _context.SaveChangesAsync();

				_logger.LogInformation("Order {OrderId} updated via chat by user {UserId}", req.OrderId, csrUserId);

				return Ok(new
				{
					id = saleOrder.Id,
					orderNumber = saleOrder.OrderNumber,
					clientName = saleOrder.ClientName,
					orderDetails = saleOrder.OrderDetails,
					deadline = saleOrder.Deadline,
					wordsOrPages = saleOrder.WordsOrPages,
					country = saleOrder.Country,
					currencyCode = saleOrder.CurrencyCode,
					foreignCurrencyAmount = saleOrder.ForeignCurrencyAmount,
					pkrAmount = saleOrder.PkrAmount,
					paymentStatus = saleOrder.PaymentStatus,
					status = saleOrder.Status,
					message = "Order updated successfully via chat."
				});
			}
			catch (JsonException)
			{
				_logger.LogWarning("AI returned unparseable JSON for update input on order {OrderId}", req.OrderId);
				return StatusCode(502, "AI returned an unexpected response format. Please try again.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to update order {OrderId} via chat", req.OrderId);
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

		public class UpdateOrderViaChatRequest
		{
			public int OrderId { get; set; }
			public string Text { get; set; } = string.Empty;
		}
	}
}
