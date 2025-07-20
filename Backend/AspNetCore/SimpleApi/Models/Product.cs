using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int? TenantId { get; set; } // Multi-tenant support

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}