using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign key for Company (Tenant)
        public int CompanyId { get; set; }
        
        // Navigation property
        public virtual Company? Company { get; set; }
    }
}