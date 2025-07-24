using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class CompanyDto
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

        [MaxLength(100)]
        public string CompanyNature { get; set; } = "General";

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class CreateCompanyDto
    {
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

        [MaxLength(100)]
        public string CompanyNature { get; set; } = "General";
    }

    public class UpdateCompanyDto
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

        [MaxLength(100)]
        public string CompanyNature { get; set; } = "General";

        public bool IsActive { get; set; } = true;
    }
}