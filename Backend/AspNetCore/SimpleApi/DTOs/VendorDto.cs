using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class VendorDto
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
        
        [StringLength(100)]
        public string? ContactPerson { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public int CompanyId { get; set; }
        
        public string? CompanyName { get; set; }
    }

    public class CreateVendorDto
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
        
        [StringLength(100)]
        public string? ContactPerson { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateVendorDto
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
        
        [StringLength(100)]
        public string? ContactPerson { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
    }
}
