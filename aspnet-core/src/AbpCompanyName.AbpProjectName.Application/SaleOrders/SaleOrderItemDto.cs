using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AbpCompanyName.AbpProjectName.SaleOrders;
using System;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Application.SaleOrders
{
    [AutoMapFrom(typeof(SaleOrderItem))]
    public class SaleOrderItemDto : EntityDto<int>
    {
        public int SaleOrderId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal LineTotal => Quantity * Price;
    }

    [AutoMapTo(typeof(SaleOrderItem))]
    public class CreateSaleOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Price { get; set; }
    }
}