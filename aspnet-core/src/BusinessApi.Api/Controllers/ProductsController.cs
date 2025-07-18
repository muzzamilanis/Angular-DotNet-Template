using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessApi.Application.DTOs;
using BusinessApi.Application.Services;

namespace BusinessApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            var result = await _productService.GetAllAsync(input);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<ProductDto>>> GetAllActive()
        {
            var result = await _productService.GetAllActiveProductsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            var result = await _productService.GetAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto input)
        {
            var result = await _productService.CreateAsync(input);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto input)
        {
            input.Id = id;
            var result = await _productService.UpdateAsync(input);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _productService.DeleteAsync(id);
        }
    }
}