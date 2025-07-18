using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class SaleOrderDto
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
        
        public List<SaleOrderItemDto> Items { get; set; } = new List<SaleOrderItemDto>();
    }

    public class CreateSaleOrderDto
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; }
        
        [StringLength(200)]
        public string? CustomerName { get; set; }
        
        [StringLength(500)]
        public string? CustomerAddress { get; set; }
        
        public List<CreateSaleOrderItemDto> Items { get; set; } = new List<CreateSaleOrderItemDto>();
    }

    public class UpdateSaleOrderDto
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
        
        public List<CreateSaleOrderItemDto> Items { get; set; } = new List<CreateSaleOrderItemDto>();
    }

    public class SaleOrderItemDto
    {
        public int Id { get; set; }
        public int SaleOrderId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public string? ProductName { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal LineTotal => Quantity * Price;
    }

    public class CreateSaleOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Price { get; set; }
    }

    public class PagedResultDto<T>
    {
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new List<T>();

        public PagedResultDto(int totalCount, List<T> items)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }

    public class PagedResultRequestDto
    {
        public int SkipCount { get; set; } = 0;
        public int MaxResultCount { get; set; } = 10;
    }
}