using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessApi.Application.DTOs;
using BusinessApi.Application.Services;

namespace BusinessApi.Api.Modules.OrderManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrdersController : ControllerBase
    {
        private readonly ISaleOrderService _saleOrderService;

        public SaleOrdersController(ISaleOrderService saleOrderService)
        {
            _saleOrderService = saleOrderService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<SaleOrderDto>>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            var result = await _saleOrderService.GetAllAsync(input);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleOrderDto>> Get(int id)
        {
            var result = await _saleOrderService.GetAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SaleOrderDto>> Create([FromBody] CreateSaleOrderDto input)
        {
            var result = await _saleOrderService.CreateAsync(input);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SaleOrderDto>> Update(int id, [FromBody] UpdateSaleOrderDto input)
        {
            input.Id = id;
            var result = await _saleOrderService.UpdateAsync(input);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _saleOrderService.DeleteAsync(id);
        }
    }
}