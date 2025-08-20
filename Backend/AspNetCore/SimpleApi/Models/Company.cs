using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [Required]
        public int BusinessNatureId { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [MaxLength(7)]
        public string? PrimaryColor { get; set; } = "#667eea";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual BusinessNature BusinessNature { get; set; } = null!;
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}