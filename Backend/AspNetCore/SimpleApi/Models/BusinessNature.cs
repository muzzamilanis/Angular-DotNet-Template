using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class BusinessNature
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? ConfigSchema { get; set; } // JSON configuration schema

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<CompanyProfile> CompanyProfiles { get; set; } = new List<CompanyProfile>();
    }
}