using System.ComponentModel.DataAnnotations;
using SimpleApi.Models;

namespace SimpleApi.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public int? ServiceTypeId { get; set; }
        public string? ServiceTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProjectDto
    {
        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ProjectDescription { get; set; }

        public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;

        public int? ServiceTypeId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateProjectDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? ProjectDescription { get; set; }

        public ProjectStatus ProjectStatus { get; set; }

        public int? ServiceTypeId { get; set; }

        public bool IsActive { get; set; }
    }

    public class ProjectListDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public int? ServiceTypeId { get; set; }
        public string? ServiceTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int RelatedSaleOrdersCount { get; set; }
    }
}