using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs.CRM
{
    public class CrmFormTemplateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string JsonSchema { get; set; } = string.Empty;

        public int BusinessNatureId { get; set; }
        public string? BusinessNatureName { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCrmFormTemplateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string JsonSchema { get; set; } = string.Empty;

        [Required]
        public int BusinessNatureId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCrmFormTemplateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string JsonSchema { get; set; } = string.Empty;

        [Required]
        public int BusinessNatureId { get; set; }

        public bool IsActive { get; set; }
    }
}