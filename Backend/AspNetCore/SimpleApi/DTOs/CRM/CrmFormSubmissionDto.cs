using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs.CRM
{
    public class CrmFormSubmissionDto
    {
        public int Id { get; set; }

        public int FormTemplateId { get; set; }
        public string? FormTemplateName { get; set; }

        [Required]
        public string SubmittedData { get; set; } = string.Empty;

        public int? LeadId { get; set; }
        public string? LeadName { get; set; }

        public int? ContactId { get; set; }
        public string? ContactName { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public bool IsActive { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class CreateCrmFormSubmissionDto
    {
        [Required]
        public int FormTemplateId { get; set; }

        [Required]
        public string SubmittedData { get; set; } = string.Empty;

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCrmFormSubmissionDto
    {
        public int Id { get; set; }

        [Required]
        public int FormTemplateId { get; set; }

        [Required]
        public string SubmittedData { get; set; } = string.Empty;

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        public bool IsActive { get; set; }
    }
}