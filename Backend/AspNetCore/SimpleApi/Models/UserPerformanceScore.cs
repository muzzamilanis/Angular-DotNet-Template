using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    /// <summary>
    /// Tracks performance evaluation scores for users against specific criteria.
    /// Links users to their performance evaluations.
    /// </summary>
    public class UserPerformanceScore
    {
        public int Id { get; set; }

        /// <summary>
        /// ID of the user being evaluated
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// ID of the performance criteria being evaluated
        /// </summary>
        [Required]
        public int PerformanceCriteriaId { get; set; }

        /// <summary>
        /// The score achieved (0-100 or based on criteria's scoring method)
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Actual value achieved (e.g., if target was 10 sales, actual might be 8)
        /// </summary>
        public decimal? AchievedValue { get; set; }

        [MaxLength(100)]
        public string? EvaluationPeriod { get; set; }

        /// <summary>
        /// ID of the user who performed the evaluation
        /// </summary>
        public int? EvaluatorUserId { get; set; }

        [MaxLength(2000)]
        public string? Comments { get; set; }

        public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// The user being evaluated
        /// </summary>
        public virtual User? User { get; set; }

        /// <summary>
        /// The performance criteria being evaluated against
        /// </summary>
        public virtual PerformanceCriteria? PerformanceCriteria { get; set; }

        /// <summary>
        /// The user who performed the evaluation
        /// </summary>
        public virtual User? Evaluator { get; set; }
    }
}
