using System.ComponentModel.DataAnnotations;
using SimpleApi.Models.CRM.Enums;

namespace SimpleApi.Models.CRM
{
    public class Lead
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Source { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public LeadStatus Status { get; set; } = LeadStatus.New;

        [Required]
        public int BusinessNatureId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual BusinessNature BusinessNature { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
        public virtual ICollection<CrmTask> Tasks { get; set; } = new List<CrmTask>();
        public virtual ICollection<CrmFormSubmission> FormSubmissions { get; set; } = new List<CrmFormSubmission>();
    }
}