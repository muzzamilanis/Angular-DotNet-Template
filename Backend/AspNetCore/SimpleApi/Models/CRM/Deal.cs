using System.ComponentModel.DataAnnotations;
using SimpleApi.Models.CRM.Enums;

namespace SimpleApi.Models.CRM
{
    public class Deal
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public decimal Value { get; set; }

        [Required]
        public DealStatus Status { get; set; } = DealStatus.Proposal;

        public DateTime? ExpectedCloseDate { get; set; }

        public DateTime? ActualCloseDate { get; set; }

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Lead? Lead { get; set; }
        public virtual Contact? Contact { get; set; }
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<CrmTask> Tasks { get; set; } = new List<CrmTask>();
    }
}