using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class BusinessNatureDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ConfigSchema { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class CompanyProfileDto
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public int BusinessNatureId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        public string? CustomFields { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public BusinessNatureDto? BusinessNature { get; set; }
    }

    public class CreateCompanyProfileDto
    {
        [Required]
        public int BusinessNatureId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        public string? CustomFields { get; set; }
    }

    public class IndustryConfigDto
    {
        public int BusinessNatureId { get; set; }
        public BusinessNatureDto? BusinessNature { get; set; }
        public List<IndustryModuleDto> Modules { get; set; } = new();
        public List<IndustryFormConfigDto> FormConfigs { get; set; } = new();
    }

    public class IndustryModuleDto
    {
        public int Id { get; set; }
        public int BusinessNatureId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? ModuleConfig { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class IndustryFormConfigDto
    {
        public int Id { get; set; }
        public int BusinessNatureId { get; set; }
        public string FormType { get; set; } = string.Empty;
        public string FieldSchema { get; set; } = string.Empty;
        public string? ValidationRules { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}