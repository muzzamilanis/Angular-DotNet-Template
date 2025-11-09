using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string AccountName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string AccountType { get; set; } = string.Empty; // Asset, Liability, Equity, Revenue, Expense

        [MaxLength(100)]
        public string? AccountCategory { get; set; } // Current Asset, Fixed Asset, etc.

        public int? ParentAccountId { get; set; }

        public int Level { get; set; } = 1;

        public decimal OpeningBalance { get; set; }

        public decimal CurrentBalance { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual Account? ParentAccount { get; set; }
        public virtual ICollection<Account> SubAccounts { get; set; } = new List<Account>();
        public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
    }
}
