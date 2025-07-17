using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.SaleOrders
{
    public class Product : Entity<int>
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