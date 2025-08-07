using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Services.CRM;

namespace SimpleApi.Controllers.CRM
{
    [Route("api/crm/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : BaseController
    {
        private readonly ICrmTaskService _taskService;

        public TasksController(ICrmTaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var tasks = await _taskService.GetAllAsync(input, companyId);
                return Ok(new { result = tasks.Items, totalCount = tasks.TotalCount, success = true });
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
        public async Task<ActionResult<object>> GetActiveTasks()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var tasks = await _taskService.GetActiveTasksAsync(companyId);
                return Ok(new { result = tasks, success = true });
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
                var task = await _taskService.GetAsync(id, companyId);
                return Ok(new { result = task, success = true });
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
        public async Task<ActionResult<object>> Create([FromBody] CreateCrmTaskDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                var companyId = GetCurrentUserCompanyId();
                var task = await _taskService.CreateAsync(input, companyId);
                return Ok(new { result = task, success = true });
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
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateCrmTaskDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var task = await _taskService.UpdateAsync(input, companyId);
                return Ok(new { result = task, success = true });
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
                await _taskService.DeleteAsync(id, companyId);
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