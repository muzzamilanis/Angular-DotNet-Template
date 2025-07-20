using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class IndustryFormConfig
    {
        public int Id { get; set; }

        public int BusinessNatureId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FormType { get; set; } = string.Empty; // "purchase", "sales", "customer", etc.

        [Required]
        public string FieldSchema { get; set; } = string.Empty; // JSON schema for dynamic forms

        public string? ValidationRules { get; set; } // JSON validation rules

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public BusinessNature BusinessNature { get; set; } = null!;
    }
}