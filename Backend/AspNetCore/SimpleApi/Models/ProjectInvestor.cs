using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class ProjectInvestor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string InvestorName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(100)]
        public string? CnicOrNtn { get; set; }

        public decimal InvestmentAmount { get; set; }

        public decimal SharePercentage { get; set; } // Percentage share in the project

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        [Required]
        public int CompanyId { get; set; }

        // Foreign keys
        [Required]
        public int ConstructionProjectId { get; set; }

        // Navigation properties
        public virtual Company Company { get; set; } = null!;
        public virtual ConstructionProject ConstructionProject { get; set; } = null!;
        public virtual ICollection<InvestorLedgerEntry> LedgerEntries { get; set; } = new List<InvestorLedgerEntry>();
    }
}
