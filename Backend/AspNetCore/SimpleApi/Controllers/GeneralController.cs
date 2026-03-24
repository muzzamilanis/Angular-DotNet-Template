using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class GeneralController : ControllerBase
	{
		private readonly ICountryService _countryService;

		public GeneralController(ICountryService countryService)
		{
			_countryService = countryService;
		}

		/// <summary>GET /api/general/countries — returns the supported country/currency reference list</summary>
		[HttpGet("countries")]
		public IActionResult GetCountries()
		{
			var countries = _countryService.GetAll()
				.Select(c => new { name = c.Name, code = c.Code, currency = c.Currency, symbol = c.Symbol });

			return Ok(countries);
		}
	}
}
