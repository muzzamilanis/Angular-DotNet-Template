using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class RegistrationDto
    {
        public UserRegistrationDto User { get; set; } = new();
        public CompanyRegistrationDto Company { get; set; } = new();
    }

    public class UserRegistrationDto
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
    }

    public class CompanyRegistrationDto
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }
    }

    public class RegistrationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
    }
}