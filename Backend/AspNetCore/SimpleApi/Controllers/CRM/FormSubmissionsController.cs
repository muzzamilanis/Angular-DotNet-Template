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
    public class FormSubmissionsController : BaseController
    {
        private readonly ICrmFormSubmissionService _formSubmissionService;

        public FormSubmissionsController(ICrmFormSubmissionService formSubmissionService)
        {
            _formSubmissionService = formSubmissionService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var submissions = await _formSubmissionService.GetAllAsync(input, companyId);
                return Ok(new { result = submissions.Items, totalCount = submissions.TotalCount, success = true });
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

        [HttpGet("by-template/{templateId}")]
        public async Task<ActionResult<object>> GetByTemplate(int templateId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var submissions = await _formSubmissionService.GetSubmissionsByTemplateAsync(templateId, companyId);
                return Ok(new { result = submissions, success = true });
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

        [HttpGet("by-lead/{leadId}")]
        public async Task<ActionResult<object>> GetByLead(int leadId)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var submissions = await _formSubmissionService.GetSubmissionsByLeadAsync(leadId, companyId);
                return Ok(new { result = submissions, success = true });
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
                var submission = await _formSubmissionService.GetAsync(id, companyId);
                return Ok(new { result = submission, success = true });
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
        public async Task<ActionResult<object>> Create([FromBody] CreateCrmFormSubmissionDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                var companyId = GetCurrentUserCompanyId();
                var submission = await _formSubmissionService.CreateAsync(input, companyId);
                return Ok(new { result = submission, success = true });
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
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateCrmFormSubmissionDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var submission = await _formSubmissionService.UpdateAsync(input, companyId);
                return Ok(new { result = submission, success = true });
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
                await _formSubmissionService.DeleteAsync(id, companyId);
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