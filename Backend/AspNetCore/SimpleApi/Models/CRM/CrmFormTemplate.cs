using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models.CRM
{
    public class CrmFormTemplate
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string JsonSchema { get; set; } = string.Empty;

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
        public virtual ICollection<CrmFormSubmission> FormSubmissions { get; set; } = new List<CrmFormSubmission>();
    }
}