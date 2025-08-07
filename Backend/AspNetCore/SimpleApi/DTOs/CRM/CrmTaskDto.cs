using System.ComponentModel.DataAnnotations;
using SimpleApi.Models.CRM.Enums;

namespace SimpleApi.DTOs.CRM
{
    public class CrmTaskDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public CrmTaskStatus Status { get; set; }

        public CrmTaskPriority Priority { get; set; }

        public int AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }

        public int? LeadId { get; set; }
        public string? LeadName { get; set; }

        public int? ContactId { get; set; }
        public string? ContactName { get; set; }

        public int? DealId { get; set; }
        public string? DealTitle { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCrmTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public CrmTaskStatus Status { get; set; } = CrmTaskStatus.Pending;

        public CrmTaskPriority Priority { get; set; } = CrmTaskPriority.Medium;

        [Required]
        public int AssignedToUserId { get; set; }

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        public int? DealId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCrmTaskDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public CrmTaskStatus Status { get; set; }

        public CrmTaskPriority Priority { get; set; }

        [Required]
        public int AssignedToUserId { get; set; }

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        public int? DealId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsActive { get; set; }
    }
}