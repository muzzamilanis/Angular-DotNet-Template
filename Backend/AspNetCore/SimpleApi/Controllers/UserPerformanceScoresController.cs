using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPerformanceScoresController : BaseController
    {
        private readonly IUserPerformanceScoreService _service;

        public UserPerformanceScoresController(IUserPerformanceScoreService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all performance scores for current user's company
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UserPerformanceScoreDto>>> GetAll()
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
        /// Get performance scores for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<UserPerformanceScoreDto>>> GetByUser(int userId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _service.GetByUserIdAsync(userId, companyId);
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
        /// Get performance score by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserPerformanceScoreDto>> Get(int id)
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
        /// Create new performance evaluation
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserPerformanceScoreDto>> Create([FromBody] CreateUserPerformanceScoreDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var evaluatorUserId = GetCurrentUserId();
                var result = await _service.CreateAsync(input, companyId, evaluatorUserId);
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

        /// <summary>
        /// Update performance score
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserPerformanceScoreDto>> Update(int id, [FromBody] UpdateUserPerformanceScoreDto input)
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
        /// Delete performance score
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
