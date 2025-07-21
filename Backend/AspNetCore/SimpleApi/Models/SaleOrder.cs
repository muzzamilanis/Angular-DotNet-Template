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
        
        public virtual ICollection<SaleOrderItem> Items { get; set; } = new List<SaleOrderItem>();
    }
}