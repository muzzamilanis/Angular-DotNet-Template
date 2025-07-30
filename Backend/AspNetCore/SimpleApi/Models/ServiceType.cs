using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class ServiceType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key for Company (Tenant)
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }
}