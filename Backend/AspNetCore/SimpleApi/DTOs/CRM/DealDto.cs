using System.ComponentModel.DataAnnotations;
using SimpleApi.Models.CRM.Enums;

namespace SimpleApi.DTOs.CRM
{
    public class DealDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public decimal Value { get; set; }

        public DealStatus Status { get; set; }

        public DateTime? ExpectedCloseDate { get; set; }

        public DateTime? ActualCloseDate { get; set; }

        public int? LeadId { get; set; }
        public string? LeadName { get; set; }

        public int? ContactId { get; set; }
        public string? ContactName { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateDealDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public decimal Value { get; set; }

        public DealStatus Status { get; set; } = DealStatus.Proposal;

        public DateTime? ExpectedCloseDate { get; set; }

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateDealDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public decimal Value { get; set; }

        public DealStatus Status { get; set; }

        public DateTime? ExpectedCloseDate { get; set; }

        public DateTime? ActualCloseDate { get; set; }

        public int? LeadId { get; set; }

        public int? ContactId { get; set; }

        public bool IsActive { get; set; }
    }
}