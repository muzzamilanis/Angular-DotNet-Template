using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models.CRM
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? JobTitle { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? LeadId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Lead? Lead { get; set; }
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
        public virtual ICollection<CrmTask> Tasks { get; set; } = new List<CrmTask>();
        public virtual ICollection<CrmFormSubmission> FormSubmissions { get; set; } = new List<CrmFormSubmission>();
    }
}