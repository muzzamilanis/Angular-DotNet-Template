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
    }
}