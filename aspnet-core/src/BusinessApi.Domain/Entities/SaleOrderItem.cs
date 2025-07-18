using System.ComponentModel.DataAnnotations;

namespace BusinessApi.Domain.Entities
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
    }
}