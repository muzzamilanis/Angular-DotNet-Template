using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenAuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public TokenAuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<object>> Authenticate([FromBody] LoginDto input)
        {
            try
            {
                var result = await _authService.LoginAsync(input);
                return Ok(new { result = result, success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IAuthService _authService;

        public SessionController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("GetCurrentLoginInformations")]
        public async Task<ActionResult<object>> GetCurrentLoginInformations()
        {
            try
            {
                var result = await _authService.GetCurrentLoginInformationAsync();
                return Ok(new { result = result, success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = new { message = ex.Message }, success = false });
            }
        }
    }
}