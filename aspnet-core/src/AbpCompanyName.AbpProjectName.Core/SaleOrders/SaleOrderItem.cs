using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbpCompanyName.AbpProjectName.SaleOrders
{
    public class SaleOrderItem : Entity<int>
    {
        [Required]
        public int SaleOrderId { get; set; }
        
        [ForeignKey("SaleOrderId")]
        public virtual SaleOrder SaleOrder { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal LineTotal => Quantity * Price;
    }
}