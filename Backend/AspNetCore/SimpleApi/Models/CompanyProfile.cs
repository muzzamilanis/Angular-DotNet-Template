using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class CompanyProfile
    {
        public int Id { get; set; }

        public int TenantId { get; set; } // Links to tenant system

        public int BusinessNatureId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        public string? CustomFields { get; set; } // JSON for industry-specific fields

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public BusinessNature BusinessNature { get; set; } = null!;
    }
}