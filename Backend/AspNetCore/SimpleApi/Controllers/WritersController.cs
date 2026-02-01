using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;
using SimpleApi.Models;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WritersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IEmailService _emailService;

        public WritersController(IUserService userService, ISaleOrderService saleOrderService, IEmailService emailService)
        {
            _userService = userService;
            _saleOrderService = saleOrderService;
            _emailService = emailService;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<UserDto>>> GetWriters()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var writers = await _userService.GetWritersAsync(companyId);
                return Ok(writers);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("assignments")]
        public async Task<ActionResult<List<WriterAssignmentDto>>> GetAssignments()
        {
            try
            {
                var userId = GetCurrentUserId();
                var assignments = await _saleOrderService.GetWriterAssignmentsAsync(userId);
                return Ok(assignments);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<List<UserDto>>> GetAvailableWriters()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var availableWriters = await _userService.GetAvailableWritersAsync(companyId);
                return Ok(availableWriters);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("submit")]
        public async Task<ActionResult> SubmitWork([FromForm] WriterSubmissionDto submission, IFormFile? submittedFile)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _saleOrderService.SubmitWriterWorkAsync(submission, submittedFile, userId);
                
                // Send email notification to Super Admin
                await SendSubmissionNotificationAsync(submission);
                
                return Ok(new { message = "Work submitted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task SendSubmissionNotificationAsync(WriterSubmissionDto submission)
        {
            try
            {
                var saleOrder = await _saleOrderService.GetAsync(submission.SaleOrderId, GetCurrentUserCompanyId());
                var currentUser = await _userService.GetUserByIdAsync(GetCurrentUserId());

                var subject = submission.SubmissionStatus == "Completed"
                    ? $"Writer has submitted work for Sale Order {saleOrder.OrderNumber}"
                    : $"Writer requires clarification/help for Sale Order {saleOrder.OrderNumber}";

                var body = $@"
                    <h3>{subject}</h3>
                    <p><strong>Writer:</strong> {currentUser.Name}</p>
                    <p><strong>Order Number:</strong> {saleOrder.OrderNumber}</p>
                    <p><strong>Client:</strong> {saleOrder.ClientName}</p>
                    <p><strong>Status:</strong> {submission.SubmissionStatus}</p>
                    <p><strong>Submission Date:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC</p>
                    {(submission.SubmissionStatus == "Completed" ? "<p>Please review and mark as complete if satisfactory.</p>" : "<p>Please provide clarification to the writer.</p>")}
                ";

                // Get Super Admin emails for notification
                var superAdmins = await _userService.GetSuperAdminsAsync(GetCurrentUserCompanyId());
                foreach (var admin in superAdmins)
                {
                    await _emailService.SendEmailAsync(admin.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the submission
                Console.WriteLine($"Failed to send notification email: {ex.Message}");
            }
        }

        [HttpGet("assignments/{saleOrderId}/instructions")]
        public async Task<ActionResult<List<OrderInstructionDto>>> GetOrderInstructions(int saleOrderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var companyId = GetCurrentUserCompanyId();

                // Verify that the writer is assigned to this order
                var assignment = await _saleOrderService.GetWriterAssignmentsAsync(userId);
                if (!assignment.Any(a => a.Id == saleOrderId))
                {
                    return Forbid("Access denied. You are not assigned to this order.");
                }

                var instructions = await _saleOrderService.GetOrderInstructionsAsync(saleOrderId, companyId);
                return Ok(instructions);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("assignments/{saleOrderId}/instructions")]
        public async Task<ActionResult<OrderInstructionDto>> AddWriterResponse(int saleOrderId, [FromBody] CreateOrderInstructionDto instruction)
        {
            try
            {
                var userId = GetCurrentUserId();
                var companyId = GetCurrentUserCompanyId();

                // Verify that the writer is assigned to this order
                var assignment = await _saleOrderService.GetWriterAssignmentsAsync(userId);
                if (!assignment.Any(a => a.Id == saleOrderId))
                {
                    return Forbid("Access denied. You are not assigned to this order.");
                }

                // Ensure the instruction is for the correct sale order
                instruction.SaleOrderId = saleOrderId;

                var result = await _saleOrderService.AddOrderInstructionAsync(instruction, userId, false, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("assignments/{saleOrderId}/instructions-with-attachment")]
        public async Task<ActionResult<OrderInstructionDto>> AddWriterResponseWithAttachment(int saleOrderId, [FromForm] CreateOrderInstructionDto instruction, IFormFile? attachmentFile)
        {
            try
            {
                var userId = GetCurrentUserId();
                var companyId = GetCurrentUserCompanyId();

                // Verify that the writer is assigned to this order
                var assignment = await _saleOrderService.GetWriterAssignmentsAsync(userId);
                if (!assignment.Any(a => a.Id == saleOrderId))
                {
                    return Forbid("Access denied. You are not assigned to this order.");
                }

                // Ensure the instruction is for the correct sale order
                instruction.SaleOrderId = saleOrderId;

                var result = await _saleOrderService.AddOrderInstructionWithAttachmentAsync(instruction, userId, false, companyId, attachmentFile);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{writerId}/workload")]
        public async Task<ActionResult<WriterWorkloadDto>> GetWriterWorkload(int writerId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var workload = await _saleOrderService.GetWriterWorkloadAsync(writerId, companyId);
                return Ok(workload);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}