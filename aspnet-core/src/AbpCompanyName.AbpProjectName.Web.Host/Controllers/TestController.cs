using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbpCompanyName.AbpProjectName.Application.SaleOrders;
using AbpCompanyName.AbpProjectName.SaleOrders;
using Microsoft.AspNetCore.Mvc;

namespace AbpCompanyName.AbpProjectName.Web.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ProductAppService _productAppService;
        private readonly SaleOrderAppService _saleOrderAppService;

        public TestController(ProductAppService productAppService, SaleOrderAppService saleOrderAppService)
        {
            _productAppService = productAppService;
            _saleOrderAppService = saleOrderAppService;
        }

        [HttpGet("test-saleorder")]
        public async Task<IActionResult> TestSaleOrder()
        {
            try
            {
                // Get products for dropdown
                var products = await _productAppService.GetAllActiveProductsAsync();
                
                if (products.Count == 0)
                {
                    return Ok(new { Message = "No products found. Please run the migrator to seed initial data." });
                }

                // Create a test sale order
                var createSaleOrderDto = new CreateSaleOrderDto
                {
                    OrderNumber = "SO-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                    OrderDate = DateTime.Now,
                    CustomerName = "Test Customer",
                    CustomerAddress = "123 Test Street, Test City",
                    Items = new List<CreateSaleOrderItemDto>
                    {
                        new CreateSaleOrderItemDto
                        {
                            ProductId = products[0].Id,
                            Quantity = 2,
                            Date = DateTime.Now,
                            Price = products[0].Price
                        }
                    }
                };

                var createdSaleOrder = await _saleOrderAppService.CreateAsync(createSaleOrderDto);

                return Ok(new 
                { 
                    Message = "Test successful!",
                    Products = products,
                    CreatedSaleOrder = createdSaleOrder
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}