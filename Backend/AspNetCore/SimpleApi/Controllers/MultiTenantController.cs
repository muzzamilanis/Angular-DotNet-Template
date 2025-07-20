using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessNaturesController : ControllerBase
    {
        private readonly IBusinessNatureService _businessNatureService;

        public BusinessNaturesController(IBusinessNatureService businessNatureService)
        {
            _businessNatureService = businessNatureService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BusinessNatureDto>>> GetAll()
        {
            var businessNatures = await _businessNatureService.GetAllAsync();
            return Ok(businessNatures);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessNatureDto>> GetById(int id)
        {
            var businessNature = await _businessNatureService.GetByIdAsync(id);
            if (businessNature == null)
            {
                return NotFound();
            }
            return Ok(businessNature);
        }

        [HttpGet("{id}/config")]
        public async Task<ActionResult<IndustryConfigDto>> GetIndustryConfig(int id)
        {
            var config = await _businessNatureService.GetIndustryConfigAsync(id);
            if (config == null)
            {
                return NotFound();
            }
            return Ok(config);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<CompanyProfileDto>> GetProfile()
        {
            // For now, use a fixed tenant ID. In a real implementation, 
            // this would come from the authenticated user's context
            var tenantId = GetTenantIdFromClaims();
            if (tenantId == null)
            {
                return BadRequest("No tenant context found");
            }

            var profile = await _companyService.GetByTenantIdAsync(tenantId.Value);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }

        [HttpPost("profile")]
        public async Task<ActionResult<CompanyProfileDto>> CreateProfile([FromBody] CreateCompanyProfileDto dto)
        {
            var tenantId = GetTenantIdFromClaims();
            if (tenantId == null)
            {
                return BadRequest("No tenant context found");
            }

            try
            {
                var profile = await _companyService.CreateAsync(dto, tenantId.Value);
                return CreatedAtAction(nameof(GetProfile), profile);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create company profile: {ex.Message}");
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult<CompanyProfileDto>> UpdateProfile([FromBody] CreateCompanyProfileDto dto)
        {
            var tenantId = GetTenantIdFromClaims();
            if (tenantId == null)
            {
                return BadRequest("No tenant context found");
            }

            var profile = await _companyService.UpdateAsync(tenantId.Value, dto);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }

        private int? GetTenantIdFromClaims()
        {
            // For demo purposes, return a fixed tenant ID
            // In a real implementation, this would extract tenant ID from JWT claims
            var tenantClaim = User.Claims.FirstOrDefault(c => c.Type == "tenantId");
            if (tenantClaim != null && int.TryParse(tenantClaim.Value, out int tenantId))
            {
                return tenantId;
            }
            
            // Return demo tenant ID for testing
            return 1;
        }
    }
}