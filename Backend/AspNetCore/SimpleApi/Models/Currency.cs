using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Currency
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
    }
}