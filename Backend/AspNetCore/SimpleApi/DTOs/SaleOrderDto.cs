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

        // Project relationship
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }

        // Digital Marketing specific fields
        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? ClientName { get; set; }

        public DateTime? Deadline { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public decimal? AmountReceived { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        public string? AssignedTo { get; set; }

        [StringLength(100)]
        public string? AcademicLevel { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }
        
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

        // Project relationship
        public int? ProjectId { get; set; }

        // Digital Marketing specific fields
        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? ClientName { get; set; }

        public DateTime? Deadline { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public decimal? AmountReceived { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        public string? AssignedTo { get; set; }

        [StringLength(100)]
        public string? AcademicLevel { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }
        
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

        // Project relationship
        public int? ProjectId { get; set; }

        // Digital Marketing specific fields
        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? ClientName { get; set; }

        public DateTime? Deadline { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public decimal? AmountReceived { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        public string? AssignedTo { get; set; }

        [StringLength(100)]
        public string? AcademicLevel { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }
        
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

        // Digital Marketing specific fields
        [StringLength(200)]
        public string? WorkTitle { get; set; }

        [StringLength(100)]
        public string? PaperType { get; set; }

        public int? WordCount { get; set; }

        public int? PageCount { get; set; }

        public DateTime? ItemDeadline { get; set; }
    }

    public class CreateSaleOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Price { get; set; }

        // Digital Marketing specific fields
        [StringLength(200)]
        public string? WorkTitle { get; set; }

        [StringLength(100)]
        public string? PaperType { get; set; }

        public int? WordCount { get; set; }

        public int? PageCount { get; set; }

        public DateTime? ItemDeadline { get; set; }
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
        public string? Filter { get; set; }
        public string? Sorting { get; set; }
    }
}