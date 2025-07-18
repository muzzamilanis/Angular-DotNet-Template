using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AbpCompanyName.AbpProjectName.SaleOrders;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Application.SaleOrders
{
    [AutoMapFrom(typeof(Customer))]
    public class CustomerDto : EntityDto<int>
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Address { get; set; }
        
        [StringLength(50)]
        public string Phone { get; set; }
        
        [StringLength(200)]
        public string Email { get; set; }
        
        public bool IsActive { get; set; }
    }

    [AutoMapTo(typeof(Customer))]
    public class CreateCustomerDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Address { get; set; }
        
        [StringLength(50)]
        public string Phone { get; set; }
        
        [StringLength(200)]
        public string Email { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    [AutoMapTo(typeof(Customer))]
    public class UpdateCustomerDto : IEntityDto<int>
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Address { get; set; }
        
        [StringLength(50)]
        public string Phone { get; set; }
        
        [StringLength(200)]
        public string Email { get; set; }
        
        public bool IsActive { get; set; }
    }
}