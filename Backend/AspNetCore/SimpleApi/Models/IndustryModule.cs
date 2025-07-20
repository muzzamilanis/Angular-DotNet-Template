using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class IndustryModule
    {
        public int Id { get; set; }

        public int BusinessNatureId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ModuleName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? DisplayName { get; set; }

        public string? ModuleConfig { get; set; } // JSON configuration

        public bool IsEnabled { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public BusinessNature BusinessNature { get; set; } = null!;
    }
}