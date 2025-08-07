using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Services.CRM;

namespace SimpleApi.Controllers.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DealsController : BaseController
    {
        private readonly IDealService _dealService;

        public DealsController(IDealService dealService)
        {
            _dealService = dealService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var deals = await _dealService.GetAllAsync(input, companyId);
                return Ok(new { result = deals.Items, totalCount = deals.TotalCount, success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = new { message = ex.Message }, success = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<object>> GetActiveDeals()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var deals = await _dealService.GetActiveDealsAsync(companyId);
                return Ok(new { result = deals, success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = new { message = ex.Message }, success = false });
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
                var companyId = GetCurrentUserCompanyId();
                var deal = await _dealService.GetAsync(id, companyId);
                return Ok(new { result = deal, success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = new { message = ex.Message }, success = false });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = new { message = ex.Message }, success = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] CreateDealDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                var companyId = GetCurrentUserCompanyId();
                var deal = await _dealService.CreateAsync(input, companyId);
                return Ok(new { result = deal, success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = new { message = ex.Message }, success = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateDealDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var deal = await _dealService.UpdateAsync(input, companyId);
                return Ok(new { result = deal, success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = new { message = ex.Message }, success = false });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = new { message = ex.Message }, success = false });
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
                var companyId = GetCurrentUserCompanyId();
                await _dealService.DeleteAsync(id, companyId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = new { message = ex.Message }, success = false });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = new { message = ex.Message }, success = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }
    }
}