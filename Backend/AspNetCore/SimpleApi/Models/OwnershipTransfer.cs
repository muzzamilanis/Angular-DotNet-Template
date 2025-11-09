using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class OwnershipTransfer
    {
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TransferNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FromOwner { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ToOwner { get; set; } = string.Empty;

        public DateTime TransferDate { get; set; }

        [MaxLength(50)]
        public string? TransferType { get; set; } = "Sale"; // Sale, Gift, Inheritance, Court Order

        public decimal? TransferValue { get; set; }

        [MaxLength(200)]
        public string? LegalDocumentNumber { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Property Property { get; set; } = null!;
    }
}
