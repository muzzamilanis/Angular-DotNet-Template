using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;
using System.Security.Claims;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : BaseController
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyNatureService _companyNatureService;

        public CompaniesController(ICompanyService companyService, ICompanyNatureService companyNatureService)
        {
            _companyService = companyService;
            _companyNatureService = companyNatureService;
        }

        private bool GetCurrentUserIsSuperAdmin()
        {
            var isSuperAdminClaim = User.FindFirst("IsSuperAdmin")?.Value;
            return bool.TryParse(isSuperAdminClaim, out bool isSuperAdmin) && isSuperAdmin;
        }

        [HttpGet("current")]
        public async Task<ActionResult<CompanyDto>> GetCurrentCompany()
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _companyService.GetCurrentUserCompanyAsync(companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
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

		[HttpGet()]
		public async Task<ActionResult<List<CompanyDto>>> Get()
		{
			try
			{
				// Only Super Admins can access any company by ID
				if (!GetCurrentUserIsSuperAdmin())
				{
					return Forbid("Only Super Admins can access company details by ID.");
				}

				var result = await _companyService.Get();
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

		[HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> Get(int id)
        {
            try
            {
                // Only Super Admins can access any company by ID
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can access company details by ID.");
                }

                var result = await _companyService.GetByIdAsync(id);
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

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto input)
        {
            try
            {
                // Only Super Admins can create companies
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only Super Admins can create companies.");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _companyService.CreateAsync(input);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyDto>> Update(int id, [FromBody] UpdateCompanyDto input)
        {
            try
            {
                // Super Admins can update any company, regular users can only update their own company
                if (!GetCurrentUserIsSuperAdmin())
                {
                    var userCompanyId = GetCurrentUserCompanyId();
                    if (id != userCompanyId)
                    {
                        return Forbid("You can only update your own company.");
                    }
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.Id = id;
                var result = await _companyService.UpdateAsync(input);
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

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            try
            {
                // Only Super Admins can delete companies
                if (!GetCurrentUserIsSuperAdmin())
                {
                    throw new UnauthorizedAccessException("Only Super Admins can delete companies.");
                }

                await _companyService.DeleteAsync(id);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost("upload-logo")]
        public async Task<ActionResult<string>> UploadLogo([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file provided");
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type. Only image files are allowed.");
                }

                // Validate file size (5MB max)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("File size too large. Maximum 5MB allowed.");
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logos");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the relative URL
                var logoUrl = fileName;
                return Ok(new { logoUrl });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading file: {ex.Message}");
            }
        }

        [HttpGet("logos/{fileName}")]
        public IActionResult GetLogo(string fileName)
        {
            try
            {
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logos", fileName);
                
                if (!System.IO.File.Exists(logoPath))
                {
                    return NotFound();
                }

                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = fileExtension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".svg" => "image/svg+xml",
                    _ => "application/octet-stream"
                };

                var fileBytes = System.IO.File.ReadAllBytes(logoPath);
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving logo: {ex.Message}");
            }
        }
    }
}