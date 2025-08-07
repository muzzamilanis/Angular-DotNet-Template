using System.ComponentModel.DataAnnotations;
using SimpleApi.Models.CRM.Enums;

namespace SimpleApi.DTOs.CRM
{
    public class LeadDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Source { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public LeadStatus Status { get; set; }

        public int BusinessNatureId { get; set; }
        public string? BusinessNatureName { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateLeadDto
    {
        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Source { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public LeadStatus Status { get; set; } = LeadStatus.New;

        [Required]
        public int BusinessNatureId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateLeadDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Source { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public LeadStatus Status { get; set; }

        [Required]
        public int BusinessNatureId { get; set; }

        public bool IsActive { get; set; }
    }
}