using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTypesController : BaseController
    {
        private readonly IServiceTypeService _serviceTypeService;

        public ServiceTypesController(IServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ServiceTypeDto>>> GetServiceTypes([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var company_id = GetCurrentUserCompanyId();
                var result = await _serviceTypeService.GetServiceTypesAsync(input, company_id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceTypeDto>> GetServiceType(int id)
        {
            try
            {
                var result = await _serviceTypeService.GetServiceTypeAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ServiceTypeDto>> CreateServiceType(CreateServiceTypeDto input)
        {
            try
            {
				input.CompanyId = GetCurrentUserCompanyId();
				var result = await _serviceTypeService.CreateServiceTypeAsync(input);
                return CreatedAtAction(nameof(GetServiceType), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceTypeDto>> UpdateServiceType(int id, CreateServiceTypeDto input)
        {
            try
            {
				input.CompanyId = GetCurrentUserCompanyId();
				var result = await _serviceTypeService.UpdateServiceTypeAsync(id, input);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceType(int id)
        {
            try
            {
                await _serviceTypeService.DeleteServiceTypeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}