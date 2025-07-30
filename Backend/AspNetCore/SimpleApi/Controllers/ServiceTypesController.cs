using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTypesController : ControllerBase
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
                var result = await _serviceTypeService.GetServiceTypesAsync(input);
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

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<List<ServiceTypeDto>>> GetServiceTypesByCompany(int companyId)
        {
            try
            {
                var result = await _serviceTypeService.GetServiceTypesByCompanyAsync(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ServiceTypeDto>> CreateServiceType(CreateServiceTypeDto input)
        {
            try
            {
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