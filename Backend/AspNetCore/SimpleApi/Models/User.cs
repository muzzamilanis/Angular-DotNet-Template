using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class User
    {
        public int Id { get; set; }

        public int? TenantId { get; set; } // Null for host users

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? FirstName { get; set; }

        [MaxLength(200)]
        public string? LastName { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsEmailConfirmed { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public CompanyProfile? CompanyProfile { get; set; }
    }
}