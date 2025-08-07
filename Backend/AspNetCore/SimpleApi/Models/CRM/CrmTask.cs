using System.ComponentModel.DataAnnotations;
using SimpleApi.Models.CRM.Enums;

namespace SimpleApi.Models.CRM
{
    public class CrmTask
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public CrmTaskStatus Status { get; set; } = CrmTaskStatus.Pending;

        [Required]
        public CrmTaskPriority Priority { get; set; } = CrmTaskPriority.Medium;

        [Required]
        public int AssignedToUserId { get; set; }

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        public int? DealId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User AssignedToUser { get; set; } = null!;
        public virtual Lead? Lead { get; set; }
        public virtual Contact? Contact { get; set; }
        public virtual Deal? Deal { get; set; }
        public virtual Company Company { get; set; } = null!;
    }
}