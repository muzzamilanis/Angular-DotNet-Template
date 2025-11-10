using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class InvestorLedgerEntry
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntryNumber { get; set; } = string.Empty;

        public DateTime EntryDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string EntryType { get; set; } = string.Empty; // Debit, Credit

        public decimal Amount { get; set; }

        public decimal Balance { get; set; } // Running balance

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Foreign keys
        [Required]
        public int ProjectInvestorId { get; set; }

        [Required]
        public int ConstructionProjectId { get; set; }

        public int? ProjectPaymentId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual ProjectInvestor ProjectInvestor { get; set; } = null!;
        public virtual ConstructionProject ConstructionProject { get; set; } = null!;
        public virtual ProjectPayment? ProjectPayment { get; set; }
    }
}
