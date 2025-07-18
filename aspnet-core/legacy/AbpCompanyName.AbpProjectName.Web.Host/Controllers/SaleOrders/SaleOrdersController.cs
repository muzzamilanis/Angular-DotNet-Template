using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Controllers;
using AbpCompanyName.AbpProjectName.Application.SaleOrders;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Web.Host.Controllers.SaleOrders
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrdersController : AbpController
    {
        private readonly SaleOrderAppService _saleOrderAppService;

        public SaleOrdersController(SaleOrderAppService saleOrderAppService)
        {
            _saleOrderAppService = saleOrderAppService;
        }

        [HttpGet]
        public async Task<PagedResultDto<SaleOrderDto>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            return await _saleOrderAppService.GetAllAsync(input);
        }

        [HttpGet("{id}")]
        public async Task<SaleOrderDto> Get(int id)
        {
            return await _saleOrderAppService.GetAsync(new EntityDto<int> { Id = id });
        }

        [HttpPost]
        public async Task<SaleOrderDto> Create([FromBody] CreateSaleOrderDto input)
        {
            return await _saleOrderAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public async Task<SaleOrderDto> Update(int id, [FromBody] UpdateSaleOrderDto input)
        {
            input.Id = id;
            return await _saleOrderAppService.UpdateAsync(input);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _saleOrderAppService.DeleteAsync(new EntityDto<int> { Id = id });
        }
    }
}