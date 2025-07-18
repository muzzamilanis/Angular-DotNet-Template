using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Customer
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

        public bool IsActive { get; set; } = true;
    }
}