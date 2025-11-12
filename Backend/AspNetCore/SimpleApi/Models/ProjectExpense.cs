using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class ProjectExpense
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ExpenseNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ExpenseCategory { get; set; } = string.Empty; // Material, Labor, Equipment, Services, etc.

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; }

        [MaxLength(100)]
        public string? VendorName { get; set; }

        [MaxLength(100)]
        public string? InvoiceNumber { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Cash, Bank Transfer, Cheque

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

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
        public virtual Voucher? Voucher { get; set; }
    }
}
