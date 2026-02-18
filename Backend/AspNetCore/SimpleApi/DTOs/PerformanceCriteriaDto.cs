using System.ComponentModel.DataAnnotations;
using SimpleApi.Models;

namespace SimpleApi.DTOs
{
    public class PerformanceCriteriaDto
    {
        public int Id { get; set; }
        public UserRole UserRole { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal WeightPercentage { get; set; }
        public string? MeasurementMethod { get; set; }
        public string? ScoringMethod { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? MinimumThreshold { get; set; }
        public decimal? MaximumThreshold { get; set; }
        public bool IsActive { get; set; }
        public bool CreatedByAI { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class CreatePerformanceCriteriaDto
    {
        [Required]
        public UserRole UserRole { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [Range(0, 100, ErrorMessage = "Weight percentage must be between 0 and 100")]
        public decimal WeightPercentage { get; set; } = 100;

        [MaxLength(200)]
        public string? MeasurementMethod { get; set; }

        [MaxLength(200)]
        public string? ScoringMethod { get; set; }

        public decimal? TargetValue { get; set; }

        public decimal? MinimumThreshold { get; set; }

        public decimal? MaximumThreshold { get; set; }

        public bool CreatedByAI { get; set; } = false;
    }

    public class UpdatePerformanceCriteriaDto
    {
        public int Id { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [Range(0, 100, ErrorMessage = "Weight percentage must be between 0 and 100")]
        public decimal WeightPercentage { get; set; }

        [MaxLength(200)]
        public string? MeasurementMethod { get; set; }

        [MaxLength(200)]
        public string? ScoringMethod { get; set; }

        public decimal? TargetValue { get; set; }

        public decimal? MinimumThreshold { get; set; }

        public decimal? MaximumThreshold { get; set; }

        public bool IsActive { get; set; }

        public bool CreatedByAI { get; set; }
    }
}
