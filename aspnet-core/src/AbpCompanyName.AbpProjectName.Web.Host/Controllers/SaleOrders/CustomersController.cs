using Abp.AspNetCore.Mvc.Controllers;
using Abp.Web.Models;
using AbpCompanyName.AbpProjectName.Application.SaleOrders;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Web.Host.Controllers.SaleOrders
{
    [Route("api/[controller]")]
    public class CustomersController : AbpController
    {
        private readonly CustomerAppService _customerAppService;

        public CustomersController(CustomerAppService customerAppService)
        {
            _customerAppService = customerAppService;
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var customers = await _customerAppService.GetAllAsync(new Abp.Application.Services.Dto.PagedAndSortedResultRequestDto());
            return Json(new AjaxResponse(customers.Items));
        }

        [HttpGet("active")]
        public async Task<JsonResult> GetActiveCustomers()
        {
            var customers = await _customerAppService.GetActiveCustomers();
            return Json(new AjaxResponse(customers.Items));
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> Get(int id)
        {
            var customer = await _customerAppService.GetAsync(new Abp.Application.Services.Dto.EntityDto<int>(id));
            return Json(new AjaxResponse(customer));
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] CreateCustomerDto input)
        {
            var customer = await _customerAppService.CreateAsync(input);
            return Json(new AjaxResponse(customer));
        }

        [HttpPut("{id}")]
        public async Task<JsonResult> Update(int id, [FromBody] UpdateCustomerDto input)
        {
            input.Id = id;
            var customer = await _customerAppService.UpdateAsync(input);
            return Json(new AjaxResponse(customer));
        }

        [HttpDelete("{id}")]
        public async Task<JsonResult> Delete(int id)
        {
            await _customerAppService.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<int>(id));
            return Json(new AjaxResponse());
        }
    }
}