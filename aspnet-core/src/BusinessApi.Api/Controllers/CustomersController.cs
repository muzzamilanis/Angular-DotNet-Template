using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessApi.Application.DTOs;
using BusinessApi.Application.Services;

namespace BusinessApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var customers = await _customerService.GetAllAsync(input);
                return Ok(new { result = customers.Items, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<object>> GetActiveCustomers()
        {
            try
            {
                var customers = await _customerService.GetActiveCustomersAsync();
                return Ok(new { result = customers, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id)
        {
            try
            {
                var customer = await _customerService.GetAsync(id);
                return Ok(new { result = customer, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] CreateCustomerDto input)
        {
            try
            {
                var customer = await _customerService.CreateAsync(input);
                return Ok(new { result = customer, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateCustomerDto input)
        {
            try
            {
                input.Id = id;
                var customer = await _customerService.UpdateAsync(input);
                return Ok(new { result = customer, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> Delete(int id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }
    }
}