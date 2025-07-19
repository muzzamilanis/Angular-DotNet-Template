using Microsoft.AspNetCore.Mvc;

namespace BusinessApi.Api.Modules.Shared.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> GetHealth()
        {
            return Ok(new { 
                status = "healthy", 
                message = "Modular Monolith API is running",
                modules = new[] { "CustomerManagement", "ProductManagement", "OrderManagement", "Shared" },
                timestamp = DateTime.UtcNow
            });
        }
    }
}