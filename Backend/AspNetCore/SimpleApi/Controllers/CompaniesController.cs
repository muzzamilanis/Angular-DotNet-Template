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
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyNatureService _companyNatureService;

        public CompaniesController(ICompanyService companyService, ICompanyNatureService companyNatureService)
        {
            _companyService = companyService;
            _companyNatureService = companyNatureService;
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

        private bool GetCurrentUserIsSuperAdmin()
        {
            var isSuperAdminClaim = User.FindFirst("IsSuperAdmin")?.Value;
            return bool.TryParse(isSuperAdminClaim, out bool isSuperAdmin) && isSuperAdmin;
        }

        [HttpGet("current")]
        public async Task<ActionResult<CompanyDto>> GetCurrentCompany()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _companyService.GetCurrentUserCompanyAsync(companyId);
                result.CompanyNature = await _companyNatureService.GetCompanyNatureAsync(companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> Get(int id)
        {
            try
            {
                // Only Super Admins can access any company by ID
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can access company details by ID.");
                }

                var result = await _companyService.GetByIdAsync(id);
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

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto input)
        {
            try
            {
                // Only Super Admins can create companies
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can create companies.");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _companyService.CreateAsync(input);
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
        public async Task<ActionResult<CompanyDto>> Update(int id, [FromBody] UpdateCompanyDto input)
        {
            try
            {
                // Super Admins can update any company, regular users can only update their own company
                if (!GetCurrentUserIsSuperAdmin())
                {
                    var userCompanyId = GetCurrentUserCompanyId();
                    if (id != userCompanyId)
                    {
                        return Forbid("You can only update your own company.");
                    }
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.Id = id;
                var result = await _companyService.UpdateAsync(input);
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
        public async Task Delete(int id)
        {
            try
            {
                // Only Super Admins can delete companies
                if (!GetCurrentUserIsSuperAdmin())
                {
                    throw new UnauthorizedAccessException("Only Super Admins can delete companies.");
                }

                await _companyService.DeleteAsync(id);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}