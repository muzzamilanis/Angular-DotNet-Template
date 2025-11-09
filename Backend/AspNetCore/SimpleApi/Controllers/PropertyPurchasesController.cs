using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyPurchasesController : BaseController
    {
        private readonly IPropertyPurchaseService _propertyPurchaseService;

        public PropertyPurchasesController(IPropertyPurchaseService propertyPurchaseService)
        {
            _propertyPurchaseService = propertyPurchaseService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<PropertyPurchaseDto>>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _propertyPurchaseService.GetAllAsync(input, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyPurchaseDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _propertyPurchaseService.GetAsync(id, companyId);
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
        public async Task<ActionResult<PropertyPurchaseDto>> Create([FromBody] CreatePropertyPurchaseDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var userId = GetCurrentUserId();
                var result = await _propertyPurchaseService.CreateAsync(input, companyId, userId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PropertyPurchaseDto>> Update(int id, [FromBody] UpdatePropertyPurchaseDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var result = await _propertyPurchaseService.UpdateAsync(input, companyId);
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _propertyPurchaseService.DeleteAsync(id, companyId);
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
