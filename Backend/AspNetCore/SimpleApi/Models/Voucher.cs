using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Voucher
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string VoucherNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VoucherType { get; set; } = string.Empty; // Journal, Cash, Bank

        public DateTime VoucherDate { get; set; }

        [MaxLength(200)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "Posted"; // Draft, Posted, Cancelled

        public bool IsPosted { get; set; } = true;

        public DateTime? PostedDate { get; set; }

        public int? PostedByUserId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual User? PostedByUser { get; set; }
        public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
