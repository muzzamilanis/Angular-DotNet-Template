using System.ComponentModel.DataAnnotations;
using SimpleApi.Models;

namespace SimpleApi.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsSuperAdmin { get; set; } = false;

        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public bool IsSuperAdmin { get; set; } = false;

        public UserRole Role { get; set; }

        public int CompanyId { get; set; }
    }

    public class UpdateUserDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsSuperAdmin { get; set; } = false;

        public UserRole Role { get; set; }

        public int CompanyId { get; set; }
    }

    public class ChangePasswordDto
    {
        public int UserId { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}