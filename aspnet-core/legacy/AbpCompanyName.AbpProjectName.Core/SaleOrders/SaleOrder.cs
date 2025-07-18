using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.SaleOrders
{
    public class SaleOrder : Entity<int>
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        [StringLength(200)]
        public string CustomerName { get; set; }
        
        [StringLength(500)]
        public string CustomerAddress { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public virtual ICollection<SaleOrderItem> Items { get; set; } = new List<SaleOrderItem>();
    }
}