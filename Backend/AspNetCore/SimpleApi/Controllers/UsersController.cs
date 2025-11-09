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
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            try
            {
                // Both Super Admins and regular users can only see users from their company
                var companyId = GetCurrentUserCompanyId();
                var companyUsers = await _userService.GetByCompanyAsync(companyId);
                return Ok(companyUsers);
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
                
                // Both Super Admins and regular users can only access users from their company
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

                // Super Admins can only create users for their own company
                var currentUserCompanyId = GetCurrentUserCompanyId();
                if (input.CompanyId != currentUserCompanyId)
                {
                    return Forbid("You can only create users for your company.");
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

                // Check if the user being updated is from the same company
                var existingUser = await _userService.GetByIdAsync(id);
                var currentUserCompanyId = GetCurrentUserCompanyId();
                if (existingUser.CompanyId != currentUserCompanyId)
                {
                    return Forbid("You can only update users from your company.");
                }

                // Ensure the user cannot be moved to a different company
                if (input.CompanyId != currentUserCompanyId)
                {
                    return Forbid("You cannot move users to a different company.");
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

                // Check if the user being deleted is from the same company
                var existingUser = await _userService.GetByIdAsync(id);
                var currentUserCompanyId = GetCurrentUserCompanyId();
                if (existingUser.CompanyId != currentUserCompanyId)
                {
                    return Forbid("You can only delete users from your company.");
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

                // Check if the user being activated is from the same company
                var existingUser = await _userService.GetByIdAsync(id);
                var currentUserCompanyId = GetCurrentUserCompanyId();
                if (existingUser.CompanyId != currentUserCompanyId)
                {
                    return Forbid("You can only activate users from your company.");
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

                // Check if the user being deactivated is from the same company
                var existingUser = await _userService.GetByIdAsync(id);
                var currentUserCompanyId = GetCurrentUserCompanyId();
                if (existingUser.CompanyId != currentUserCompanyId)
                {
                    return Forbid("You can only deactivate users from your company.");
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

                // Check if the user being updated is from the same company
                var existingUser = await _userService.GetByIdAsync(id);
                var currentUserCompanyId = GetCurrentUserCompanyId();
                if (existingUser.CompanyId != currentUserCompanyId)
                {
                    return Forbid("You can only reset passwords for users from your company.");
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