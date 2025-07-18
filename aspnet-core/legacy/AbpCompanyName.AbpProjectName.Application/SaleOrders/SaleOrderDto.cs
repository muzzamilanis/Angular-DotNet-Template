using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AbpCompanyName.AbpProjectName.SaleOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Application.SaleOrders
{
    [AutoMapFrom(typeof(SaleOrder))]
    public class SaleOrderDto : EntityDto<int>
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
        
        public List<SaleOrderItemDto> Items { get; set; } = new List<SaleOrderItemDto>();
    }

    [AutoMapTo(typeof(SaleOrder))]
    public class CreateSaleOrderDto
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        [StringLength(200)]
        public string CustomerName { get; set; }
        
        [StringLength(500)]
        public string CustomerAddress { get; set; }
        
        public List<CreateSaleOrderItemDto> Items { get; set; } = new List<CreateSaleOrderItemDto>();
    }

    [AutoMapTo(typeof(SaleOrder))]
    public class UpdateSaleOrderDto : IEntityDto<int>
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        [StringLength(200)]
        public string CustomerName { get; set; }
        
        [StringLength(500)]
        public string CustomerAddress { get; set; }
        
        public List<CreateSaleOrderItemDto> Items { get; set; } = new List<CreateSaleOrderItemDto>();
    }
}