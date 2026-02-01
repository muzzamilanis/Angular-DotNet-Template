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
    public class TeamLeadController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ISaleOrderService _saleOrderService;

        public TeamLeadController(IUserService userService, ISaleOrderService saleOrderService)
        {
            _userService = userService;
            _saleOrderService = saleOrderService;
        }

        [HttpGet("orders")]
        public async Task<ActionResult<List<TeamLeadOrderDto>>> GetOrders()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();
                var isSuperAdmin = GetCurrentUserIsSuperAdmin();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can access this resource.");
                }

                var orders = await _saleOrderService.GetTeamLeadOrdersAsync(companyId);
                return Ok(orders);
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

        [HttpGet("writers")]
        public async Task<ActionResult<List<UserDto>>> GetWriters()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();
                var isSuperAdmin = GetCurrentUserIsSuperAdmin();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can access this resource.");
                }

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

        [HttpPost("assign")]
        public async Task<ActionResult> AssignOrder([FromBody] AssignOrderDto assignment)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();
                var isSuperAdmin = GetCurrentUserIsSuperAdmin();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can assign orders.");
                }

                await _saleOrderService.AssignOrderToWriterAsync(assignment.SaleOrderId, assignment.WriterId, companyId);
                return Ok(new { message = "Order assigned successfully" });
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

        [HttpPut("assign")]
        public async Task<ActionResult> UpdateAssignment([FromBody] AssignOrderDto assignment)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();
                var isSuperAdmin = GetCurrentUserIsSuperAdmin();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can update assignments.");
                }

                await _saleOrderService.UpdateOrderAssignmentAsync(assignment.SaleOrderId, assignment.WriterId, companyId);
                return Ok(new { message = "Assignment updated successfully" });
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

        [HttpGet("orders/{saleOrderId}/instructions")]
        public async Task<ActionResult<List<OrderInstructionDto>>> GetOrderInstructions(int saleOrderId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can access this resource.");
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

        [HttpPost("orders/{saleOrderId}/instructions")]
        public async Task<ActionResult<OrderInstructionDto>> AddOrderInstruction(int saleOrderId, [FromBody] CreateOrderInstructionDto instruction)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();
                var senderId = GetCurrentUserId();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can add instructions.");
                }

                // Ensure the instruction is for the correct sale order
                instruction.SaleOrderId = saleOrderId;

                var result = await _saleOrderService.AddOrderInstructionAsync(instruction, senderId, true, companyId);
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

        [HttpPut("orders/{saleOrderId}/details")]
        public async Task<ActionResult> UpdateOrderDetails(int saleOrderId, [FromBody] UpdateOrderDetailsDto updateDto)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can update order details.");
                }

                // Ensure the update is for the correct sale order
                updateDto.Id = saleOrderId;

                await _saleOrderService.UpdateOrderDetailsAsync(updateDto, companyId);
                return Ok(new { message = "Order details updated successfully" });
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

        [HttpPost("orders/{saleOrderId}/upload-instructions")]
        public async Task<ActionResult> UploadInstructionAttachments(int saleOrderId, [FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || !files.Any())
                    return BadRequest("No files provided");

                var companyId = GetCurrentUserCompanyId();
                var currentUserRole = GetCurrentUserRole();

                // Check if user is Team Lead (Manager) only
                if (currentUserRole != UserRole.Manager && currentUserRole != UserRole.SuperAdmin)
                {
                    return Forbid("Access denied. Only authorize members can upload files.");
                }

                // Get the sale order to verify it exists and belongs to the company
                var saleOrder = await _saleOrderService.GetAsync(saleOrderId, companyId);
                if (saleOrder == null)
                    return NotFound("Sale order not found");

                // Create assets directory if it doesn't exist
                var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                if (!Directory.Exists(assetsPath))
                    Directory.CreateDirectory(assetsPath);

                var uploadedFiles = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var filePath = Path.Combine(assetsPath, fileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        uploadedFiles.Add(fileName);
                    }
                }

                return Ok(new { message = "Files uploaded successfully", filenames = uploadedFiles });
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
    }
}
