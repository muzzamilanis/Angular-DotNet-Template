using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class SaleOrder
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; }
        
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [StringLength(500)]
        public string? CustomerAddress { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        // Foreign key for Company (Tenant)
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        // Foreign key for Project (Optional - for digital marketing companies)
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        // Foreign key for ServiceType (Optional - for digital marketing companies)
        public int? ServiceTypeId { get; set; }
        public virtual ServiceType? ServiceType { get; set; }

        // Digital Marketing specific fields
        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? ClientName { get; set; }

        public DateTime? Deadline { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public decimal? AmountReceived { get; set; }

        public decimal? TotalPayment { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        [StringLength(100)]
        public string? SourceOfPayment { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        public string? AssignedTo { get; set; }

        [StringLength(1000)]
        public string? OrderDetails { get; set; }

        [StringLength(100)]
        public string? WordsOrPages { get; set; }

        [StringLength(100)]
        public string? AcademicLevel { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        // Currency-related fields for Digital Marketing companies
        [StringLength(10)]
        public string? CurrencyCode { get; set; }

        public decimal? ForeignCurrencyAmount { get; set; }

        public decimal? PkrAmount { get; set; }

        public decimal? ExchangeRateUsed { get; set; }
        
        public virtual ICollection<SaleOrderItem> Items { get; set; } = new List<SaleOrderItem>();
    }
}