using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models.CRM
{
    public class CrmFormSubmission
    {
        public int Id { get; set; }

        [Required]
        public int FormTemplateId { get; set; }

        [Required]
        public string SubmittedData { get; set; } = string.Empty;

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CrmFormTemplate FormTemplate { get; set; } = null!;
        public virtual Lead? Lead { get; set; }
        public virtual Contact? Contact { get; set; }
        public virtual Company Company { get; set; } = null!;
    }
}