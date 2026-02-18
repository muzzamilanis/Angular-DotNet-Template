using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class UserPerformanceScoreDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int PerformanceCriteriaId { get; set; }
        public string? CriteriaName { get; set; }
        public decimal Score { get; set; }
        public decimal? AchievedValue { get; set; }
        public string? EvaluationPeriod { get; set; }
        public int? EvaluatorUserId { get; set; }
        public string? EvaluatorName { get; set; }
        public string? Comments { get; set; }
        public DateTime EvaluationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class CreateUserPerformanceScoreDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int PerformanceCriteriaId { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public decimal Score { get; set; }

        public decimal? AchievedValue { get; set; }

        [MaxLength(100)]
        public string? EvaluationPeriod { get; set; }

        public int? EvaluatorUserId { get; set; }

        [MaxLength(2000)]
        public string? Comments { get; set; }
    }

    public class UpdateUserPerformanceScoreDto
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PerformanceCriteriaId { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public decimal Score { get; set; }

        public decimal? AchievedValue { get; set; }

        [MaxLength(100)]
        public string? EvaluationPeriod { get; set; }

        public int? EvaluatorUserId { get; set; }

        [MaxLength(2000)]
        public string? Comments { get; set; }
    }
}
