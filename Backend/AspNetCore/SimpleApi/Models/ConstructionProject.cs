using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public enum ConstructionProjectStatus
    {
        Planning,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }

    public class ConstructionProject
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? ProjectDescription { get; set; }

        [MaxLength(500)]
        public string? Location { get; set; }

        public ConstructionProjectStatus Status { get; set; } = ConstructionProjectStatus.Planning;

        public decimal EstimatedBudget { get; set; }

        public decimal ActualCost { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpectedEndDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        [MaxLength(100)]
        public string? ProjectManager { get; set; }

        [MaxLength(100)]
        public string? Contractor { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<ProjectClient> ProjectClients { get; set; } = new List<ProjectClient>();
        public virtual ICollection<ProjectInvestor> ProjectInvestors { get; set; } = new List<ProjectInvestor>();
        public virtual ICollection<ProjectPayment> ProjectPayments { get; set; } = new List<ProjectPayment>();
        public virtual ICollection<ProjectExpense> ProjectExpenses { get; set; } = new List<ProjectExpense>();
        public virtual ICollection<ProjectLedgerEntry> ProjectLedgerEntries { get; set; } = new List<ProjectLedgerEntry>();
    }
}
