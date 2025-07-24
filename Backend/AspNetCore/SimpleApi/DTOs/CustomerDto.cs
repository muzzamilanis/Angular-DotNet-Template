using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? Email { get; set; }
        
        public bool IsActive { get; set; }
        
        public int CompanyId { get; set; }
        
        public string? CompanyName { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? Email { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCustomerDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? Phone { get; set; }
        
        [StringLength(200)]
        public string? Email { get; set; }
        
        public bool IsActive { get; set; }
    }
}