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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
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
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _productService.GetAllAsync(input, companyId);
                return Ok(result);
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

        [HttpGet("active")]
        public async Task<ActionResult<List<ProductDto>>> GetAllActive()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _productService.GetAllActiveProductsAsync(companyId);
                return Ok(result);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _productService.GetAsync(id, companyId);
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

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var result = await _productService.CreateAsync(input, companyId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var result = await _productService.UpdateAsync(input, companyId);
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

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _productService.DeleteAsync(id, companyId);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}