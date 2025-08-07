using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs.CRM
{
    public class ContactDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? LeadId { get; set; }
        public string? LeadName { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateContactDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? LeadId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateContactDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? LeadId { get; set; }

        public bool IsActive { get; set; }
    }
}