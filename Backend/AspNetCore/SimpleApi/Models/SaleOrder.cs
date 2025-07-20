using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class SaleOrder
    {
        public int Id { get; set; }

        public int? TenantId { get; set; } // Multi-tenant support

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; }
        
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [StringLength(500)]
        public string? CustomerAddress { get; set; }
        
        public decimal TotalAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<SaleOrderItem> Items { get; set; } = new List<SaleOrderItem>();
    }
}