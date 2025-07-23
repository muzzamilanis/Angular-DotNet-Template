using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? ProjectDescription { get; set; }

        public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Foreign key for Company (Tenant)
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property for related SaleOrders
        public virtual ICollection<SaleOrder> SaleOrders { get; set; } = new List<SaleOrder>();
    }
}