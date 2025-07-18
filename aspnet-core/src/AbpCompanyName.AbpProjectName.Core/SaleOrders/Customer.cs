using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.SaleOrders
{
    public class Customer : Entity<int>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;
    }
}