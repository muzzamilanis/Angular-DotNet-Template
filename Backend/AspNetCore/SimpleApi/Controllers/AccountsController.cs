using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDto>>> GetAll()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _accountService.GetAllAsync(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("by-type/{accountType}")]
        public async Task<ActionResult<List<AccountDto>>> GetByType(string accountType)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _accountService.GetByTypeAsync(accountType, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _accountService.GetAsync(id, companyId);
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
        public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var result = await _accountService.CreateAsync(input, companyId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
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
        public async Task<ActionResult<AccountDto>> Update(int id, [FromBody] UpdateAccountDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var result = await _accountService.UpdateAsync(input, companyId);
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
                await _accountService.DeleteAsync(id, companyId);
                return NoContent();
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

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeDefaultAccounts()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _accountService.InitializeDefaultAccountsAsync(companyId);
                return Ok(new { message = "Default accounts initialized successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
