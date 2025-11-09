using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.Models;

namespace SimpleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessNaturesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BusinessNaturesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BusinessNatures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessNature>>> GetBusinessNatures()
        {
            return await _context.BusinessNatures
                .Where(bn => bn.IsActive)
                .OrderBy(bn => bn.DisplayName)
                .ToListAsync();
        }

        // GET: api/BusinessNatures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BusinessNature>> GetBusinessNature(int id)
        {
            var businessNature = await _context.BusinessNatures.FindAsync(id);

            if (businessNature == null)
            {
                return NotFound();
            }

            return businessNature;
        }
    }
}