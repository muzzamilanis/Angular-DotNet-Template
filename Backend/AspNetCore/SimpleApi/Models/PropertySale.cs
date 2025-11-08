using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class PropertySale
    {
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SaleNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string BuyerName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? BuyerAddress { get; set; }

        [MaxLength(50)]
        public string? BuyerPhone { get; set; }

        [MaxLength(200)]
        public string? BuyerEmail { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal SalePrice { get; set; }

        public decimal CostPrice { get; set; } // Original purchase/acquisition cost

        public decimal ProfitOrLoss { get; set; } // Calculated: SalePrice - CostPrice

        public decimal? AdvanceReceived { get; set; }

        public decimal? RemainingReceivable { get; set; }

        [MaxLength(50)]
        public string? PaymentStatus { get; set; } = "Pending"; // Pending, Partial, Completed

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Cash, Bank, Cheque

        [MaxLength(100)]
        public string? ChequeNumber { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Accounting - Link to voucher
        public int? VoucherId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Property Property { get; set; } = null!;
        public virtual Voucher? Voucher { get; set; }
    }
}
