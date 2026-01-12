using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Vendor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign key for Company (Tenant)
        [Required]
        public int CompanyId { get; set; }
        
        // Navigation property
        public virtual Company? Company { get; set; }
    }
}
