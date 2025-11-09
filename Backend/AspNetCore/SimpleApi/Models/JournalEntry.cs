using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        [Required]
        public int VoucherId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(10)]
        public string EntryType { get; set; } = string.Empty; // Debit, Credit

        public decimal Amount { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime EntryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Voucher Voucher { get; set; } = null!;
        public virtual Account Account { get; set; } = null!;
    }
}
