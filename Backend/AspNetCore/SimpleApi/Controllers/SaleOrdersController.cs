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
    public class SaleOrdersController : BaseController
    {
        private readonly ISaleOrderService _saleOrderService;

        public SaleOrdersController(ISaleOrderService saleOrderService)
        {
            _saleOrderService = saleOrderService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<SaleOrderDto>>> GetAll([FromQuery] PagedResultRequestDto input)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _saleOrderService.GetAllAsync(input, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleOrderDto>> Get(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                var result = await _saleOrderService.GetAsync(id, companyId);
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

        [HttpPost]
        public async Task<ActionResult<SaleOrderDto>> Create([FromBody] CreateSaleOrderDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var companyId = GetCurrentUserCompanyId();
                var result = await _saleOrderService.CreateAsync(input, companyId);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
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
        public async Task<ActionResult<SaleOrderDto>> Update(int id, [FromBody] UpdateSaleOrderDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                input.Id = id;
                var companyId = GetCurrentUserCompanyId();
                var result = await _saleOrderService.UpdateAsync(input, companyId);
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

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            try
            {
                var companyId = GetCurrentUserCompanyId();
                await _saleOrderService.DeleteAsync(id, companyId);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        [HttpPost("{id}/upload-attachments")]
        public async Task<ActionResult> UploadAttachments(int id, [FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || !files.Any())
                    return BadRequest("No files provided");

                var companyId = GetCurrentUserCompanyId();
                
                // Get the sale order
                var saleOrder = await _saleOrderService.GetAsync(id, companyId);
                if (saleOrder == null)
                    return NotFound("Sale order not found");

                // Create assets directory if it doesn't exist
                var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                if (!Directory.Exists(assetsPath))
                    Directory.CreateDirectory(assetsPath);

                var uploadedFiles = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var filePath = Path.Combine(assetsPath, fileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        uploadedFiles.Add(fileName);
                    }
                }

                // Update the sale order with the new attachment filenames
                var existingAttachments = string.IsNullOrEmpty(saleOrder.AttachmentFilenames) 
                    ? new List<string>() 
                    : saleOrder.AttachmentFilenames.Split(',').ToList();
                
                existingAttachments.AddRange(uploadedFiles);
                
                // If work was submitted by writer and attachments are being added, revert status to InProgress
                var newStatus = saleOrder.Status;
                if (saleOrder.Status == "workSubmittedByWriter")
                {
                    newStatus = "InProgress";
                }
                
                var updateDto = new UpdateSaleOrderDto
                {
                    Id = saleOrder.Id,
                    OrderNumber = saleOrder.OrderNumber,
                    OrderDate = saleOrder.OrderDate,
                    CustomerName = saleOrder.CustomerName,
                    CustomerAddress = saleOrder.CustomerAddress,
                    ProjectId = saleOrder.ProjectId,
                    ServiceTypeId = saleOrder.ServiceTypeId,
                    Code = saleOrder.Code,
                    ClientName = saleOrder.ClientName,
                    Deadline = saleOrder.Deadline,
                    Status = newStatus,
                    AmountReceived = saleOrder.AmountReceived,
                    AmountReceivedInPkr = saleOrder.AmountReceivedInPkr,
                    PaymentStatus = saleOrder.PaymentStatus,
                    SourceOfPayment = saleOrder.SourceOfPayment,
                    ContactEmail = saleOrder.ContactEmail,
                    ContactPhone = saleOrder.ContactPhone,
                    OrderDetails = saleOrder.OrderDetails,
                    WordsOrPages = saleOrder.WordsOrPages,
                    TotalPayment = saleOrder.TotalPayment,
                    AssignedTo = saleOrder.AssignedTo,
                    AttachmentFilenames = string.Join(",", existingAttachments),
                    AcademicLevel = saleOrder.AcademicLevel,
                    Country = saleOrder.Country,
                    CurrencyCode = saleOrder.CurrencyCode,
                    ForeignCurrencyAmount = saleOrder.ForeignCurrencyAmount,
                    PkrAmount = saleOrder.PkrAmount,
                    ExchangeRateUsed = saleOrder.ExchangeRateUsed,
                    Items = new List<CreateSaleOrderItemDto>() // Keep existing items unchanged
                };

                await _saleOrderService.UpdateAsync(updateDto, companyId);

                return Ok(new { message = "Files uploaded successfully", filenames = uploadedFiles });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("download/{filename}")]
        public async Task<ActionResult> DownloadFile(string filename)
        {
            try
            {
                var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
                var filePath = Path.Combine(assetsPath, filename);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found");

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var contentType = GetContentType(filename);
                return File(memory, contentType, filename);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}