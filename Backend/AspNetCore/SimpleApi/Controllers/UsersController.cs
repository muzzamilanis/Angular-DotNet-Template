using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;
using System.Security.Claims;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private bool GetCurrentUserIsSuperAdmin()
        {
            var isSuperAdminClaim = User.FindFirst("IsSuperAdmin")?.Value;
            return bool.TryParse(isSuperAdminClaim, out bool isSuperAdmin) && isSuperAdmin;
        }

        private int GetCurrentUserCompanyId()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (int.TryParse(companyIdClaim, out int companyId))
            {
                return companyId;
            }
            throw new UnauthorizedAccessException("Unable to determine user's company.");
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            try
            {
                // Only Super Admins can see all users
                if (GetCurrentUserIsSuperAdmin())
                {
                    var allUsers = await _userService.GetAllAsync();
                    return Ok(allUsers);
                }
                else
                {
                    // Regular users can only see users from their company
                    var companyId = GetCurrentUserCompanyId();
                    var companyUsers = await _userService.GetByCompanyAsync(companyId);
                    return Ok(companyUsers);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                
                // Super Admins can access any user
                if (GetCurrentUserIsSuperAdmin())
                {
                    return Ok(user);
                }
                
                // Regular users can only access users from their company
                var userCompanyId = GetCurrentUserCompanyId();
                if (user.CompanyId != userCompanyId)
                {
                    return Forbid("You can only access users from your company.");
                }
                
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto input)
        {
            try
            {
                // Only Super Admins can create users
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can create users.");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.CreateAsync(input);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto input)
        {
            try
            {
                // Only Super Admins can update users
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can update users.");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.Id = id;
                var result = await _userService.UpdateAsync(input);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Only Super Admins can delete users
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can delete users.");
                }

                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult<UserDto>> Activate(int id)
        {
            try
            {
                // Only Super Admins can activate/deactivate users
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can activate users.");
                }

                var result = await _userService.ActivateAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<UserDto>> Deactivate(int id)
        {
            try
            {
                // Only Super Admins can activate/deactivate users
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can deactivate users.");
                }

                var result = await _userService.DeactivateAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/reset-password")]
        public async Task<ActionResult> ResetPassword(int id, [FromBody] ChangePasswordDto input)
        {
            try
            {
                // Only Super Admins can reset passwords
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can reset passwords.");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.UserId = id;
                await _userService.ResetPasswordAsync(input);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}