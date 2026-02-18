using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Models;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PerformanceCriteriaController : BaseController
    {
        private readonly IPerformanceCriteriaService _service;

        public PerformanceCriteriaController(IPerformanceCriteriaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all performance criteria for current user's company
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PerformanceCriteriaDto>>> GetAll()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _service.GetAllAsync(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get performance criteria for a specific UserRole
        /// </summary>
        [HttpGet("by-role/{role}")]
        public async Task<ActionResult<List<PerformanceCriteriaDto>>> GetByRole(UserRole role)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _service.GetByUserRoleAsync(role, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get performance criteria by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PerformanceCriteriaDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _service.GetByIdAsync(id, companyId);
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

        /// <summary>
        /// Create new performance criteria
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PerformanceCriteriaDto>> Create([FromBody] CreatePerformanceCriteriaDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var result = await _service.CreateAsync(input, companyId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update performance criteria
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PerformanceCriteriaDto>> Update(int id, [FromBody] UpdatePerformanceCriteriaDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != input.Id)
                    return BadRequest("ID mismatch");

                var companyId = GetCurrentUserCompanyId();
                var result = await _service.UpdateAsync(input, companyId);
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

        /// <summary>
        /// Delete performance criteria
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _service.DeleteAsync(id, companyId);
                return NoContent();
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
    }
}
