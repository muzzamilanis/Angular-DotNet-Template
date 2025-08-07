using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.DTOs.CRM;
using SimpleApi.Services.CRM;

namespace SimpleApi.Controllers.CRM
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : BaseController
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var contacts = await _contactService.GetAllAsync(input, companyId);
                return Ok(new { result = contacts.Items, totalCount = contacts.TotalCount, success = true });
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
        public async Task<ActionResult<object>> GetActiveContacts()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var contacts = await _contactService.GetActiveContactsAsync(companyId);
                return Ok(new { result = contacts, success = true });
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
                var contact = await _contactService.GetAsync(id, companyId);
                return Ok(new { result = contact, success = true });
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
        public async Task<ActionResult<object>> Create([FromBody] CreateContactDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                var companyId = GetCurrentUserCompanyId();
                var contact = await _contactService.CreateAsync(input, companyId);
                return Ok(new { result = contact, success = true });
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
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateContactDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = new { message = "Invalid input data" }, success = false });

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var contact = await _contactService.UpdateAsync(input, companyId);
                return Ok(new { result = contact, success = true });
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
                await _contactService.DeleteAsync(id, companyId);
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