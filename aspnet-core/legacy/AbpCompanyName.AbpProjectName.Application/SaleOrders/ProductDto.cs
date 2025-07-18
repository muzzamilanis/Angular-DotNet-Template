using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AbpCompanyName.AbpProjectName.SaleOrders;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Application.SaleOrders
{
    [AutoMapFrom(typeof(Product))]
    public class ProductDto : EntityDto<int>
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public decimal Price { get; set; }
        
        public bool IsActive { get; set; }
    }

    [AutoMapTo(typeof(Product))]
    public class CreateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public decimal Price { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}