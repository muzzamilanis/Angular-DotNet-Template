using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;

namespace SimpleApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HealthController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;

		public HealthController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet("ping")]
		public IActionResult Ping()
		{
			return Ok(new { status = "Healthy", api = "Running" });
		}

		[HttpGet("dbping")]
		public async Task<IActionResult> DbPing()
		{
			try
			{
				var canConnect = await _dbContext.Database.CanConnectAsync();
				if (canConnect)
					return Ok(new { status = "Healthy", db = "Connected" });
				else
					return StatusCode(503, new { status = "Unhealthy", db = "Disconnected" });
			}
			catch (Exception ex)
			{
				return StatusCode(503, new { status = "Unhealthy", error = ex.Message });
			}
		}
	}
}
