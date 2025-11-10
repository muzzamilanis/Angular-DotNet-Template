using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public enum PaymentSource
    {
        Client,
        Investor
    }

    public class ProjectPayment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string PaymentNumber { get; set; } = string.Empty;

        public PaymentSource PaymentSource { get; set; }

        public int? ProjectClientId { get; set; }

        public int? ProjectInvestorId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Cash, Bank Transfer, Cheque

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Foreign keys
        [Required]
        public int ConstructionProjectId { get; set; }

        public int? VoucherId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual ConstructionProject ConstructionProject { get; set; } = null!;
        public virtual ProjectClient? ProjectClient { get; set; }
        public virtual ProjectInvestor? ProjectInvestor { get; set; }
        public virtual Voucher? Voucher { get; set; }
    }
}
