using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models
{
    public class SaleOrderItem
    {
        public int Id { get; set; }

        public int SaleOrderId { get; set; }
        public virtual SaleOrder? SaleOrder { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal LineTotal => Quantity * Price;

        // Digital Marketing specific fields
        [StringLength(200)]
        public string? WorkTitle { get; set; }

        [StringLength(100)]
        public string? PaperType { get; set; }

        public int? WordCount { get; set; }

        public int? PageCount { get; set; }

        public DateTime? ItemDeadline { get; set; }
    }
}