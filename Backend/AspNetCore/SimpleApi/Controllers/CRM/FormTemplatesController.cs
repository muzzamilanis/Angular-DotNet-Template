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
    public class FormTemplatesController : BaseController
    {
        private readonly ICrmFormTemplateService _formTemplateService;

        public FormTemplatesController(ICrmFormTemplateService formTemplateService)
        {
            _formTemplateService = formTemplateService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var templates = await _formTemplateService.GetAllAsync(input, companyId);
                return Ok(new { result = templates.Items, totalCount = templates.TotalCount, success = true });
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
        public async Task<ActionResult<object>> GetActiveTemplates()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var templates = await _formTemplateService.GetActiveTemplatesAsync(companyId);
                return Ok(new { result = templates, success = true });
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

        [HttpGet("by-business-nature/{businessNatureId}")]
        public async Task<ActionResult<object>> GetByBusinessNature(int businessNatureId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var templates = await _formTemplateService.GetTemplatesByBusinessNatureAsync(businessNatureId, companyId);
                return Ok(new { result = templates, success = true });
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
                var template = await _formTemplateService.GetAsync(id, companyId);
                return Ok(new { result = template, success = true });
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
        public async Task<ActionResult<object>> Create([FromBody] CreateCrmFormTemplateDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                var companyId = GetCurrentUserCompanyId();
                var template = await _formTemplateService.CreateAsync(input, companyId);
                return Ok(new { result = template, success = true });
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
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateCrmFormTemplateDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var template = await _formTemplateService.UpdateAsync(input, companyId);
                return Ok(new { result = template, success = true });
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
                await _formTemplateService.DeleteAsync(id, companyId);
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