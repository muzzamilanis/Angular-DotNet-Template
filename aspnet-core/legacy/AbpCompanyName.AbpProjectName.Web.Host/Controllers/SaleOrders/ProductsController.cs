using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Controllers;
using AbpCompanyName.AbpProjectName.Application.SaleOrders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Web.Host.Controllers.SaleOrders
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : AbpController
    {
        private readonly ProductAppService _productAppService;

        public ProductsController(ProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        [HttpGet]
        public async Task<PagedResultDto<ProductDto>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            return await _productAppService.GetAllAsync(input);
        }

        [HttpGet("active")]
        public async Task<List<ProductDto>> GetAllActive()
        {
            return await _productAppService.GetAllActiveProductsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ProductDto> Get(int id)
        {
            return await _productAppService.GetAsync(new EntityDto<int> { Id = id });
        }

        [HttpPost]
        public async Task<ProductDto> Create([FromBody] CreateProductDto input)
        {
            return await _productAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<ProductDto> Update(int id, [FromBody] ProductDto input)
        {
            input.Id = id;
            return await _productAppService.UpdateAsync(input);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _productAppService.DeleteAsync(new EntityDto<int> { Id = id });
        }
    }
}