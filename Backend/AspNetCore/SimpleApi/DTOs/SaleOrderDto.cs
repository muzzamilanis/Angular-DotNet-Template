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
        
        // Company relationship
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        // Project relationship
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }

        // ServiceType relationship
        public int? ServiceTypeId { get; set; }
        public string? ServiceTypeName { get; set; }

        // Digital Marketing specific fields
        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? ClientName { get; set; }

        public DateTime? Deadline { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public decimal? AmountReceived { get; set; }
		public decimal? AmountReceivedInPkr { get; set; }

		[StringLength(50)]
        public string? PaymentStatus { get; set; }
        [StringLength(100)]
        public string? SourceOfPayment { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        public string? AssignedTo { get; set; }



        [StringLength(2000)]
        public string? AttachmentFilenames { get; set; }

        [StringLength(500)]
        public string? SubmittedFilename { get; set; }

        [StringLength(50)]
        public string? SubmissionStatus { get; set; }

        public DateTime? SubmissionDate { get; set; }

        [StringLength(1000)]
        public string? OrderDetails { get; set; }

        [StringLength(100)]
        public string? WordsOrPages { get; set; }

        public decimal? TotalPayment { get; set; }

        [StringLength(100)]
        public string? AcademicLevel { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        // Currency-related fields for Digital Marketing companies
        [StringLength(10)]
        public string? CurrencyCode { get; set; }

        public decimal? ForeignCurrencyAmount { get; set; }

        public decimal? PkrAmount { get; set; }

        public decimal? ExchangeRateUsed { get; set; }
        
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
        // ServiceType relationship
        public int? ServiceTypeId { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? ClientName { get; set; }

        public DateTime? Deadline { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public decimal? AmountReceived { get; set; }
		public decimal? AmountReceivedInPkr { get; set; }

		[StringLength(50)]
        public string? PaymentStatus { get; set; }
        [StringLength(100)]
        public string? SourceOfPayment { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        public string? AssignedTo { get; set; }


        [StringLength(2000)]
        public string? AttachmentFilenames { get; set; }

        [StringLength(1000)]
        public string? OrderDetails { get; set; }

        [StringLength(100)]
        public string? WordsOrPages { get; set; }

        public decimal? TotalPayment { get; set; }

        [StringLength(100)]
        public string? AcademicLevel { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        // Currency-related fields for Digital Marketing companies
        [StringLength(10)]
        public string? CurrencyCode { get; set; }

        public decimal? ForeignCurrencyAmount { get; set; }

        public decimal? PkrAmount { get; set; }

        public decimal? ExchangeRateUsed { get; set; }

        /// <summary>
        /// The original natural language text typed by the CSR in the chat.
        /// Persisted as an OrderInstruction for audit and dispute resolution.
        /// </summary>
        [StringLength(2000)]
        public string? CsrChatText { get; set; }

        public List<CreateSaleOrderItemDto> Items { get; set; } = new List<CreateSaleOrderItemDto>();
    }

    public class UpdateSaleOrderDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        // ServiceType relationship
        public int? ServiceTypeId { get; set; }
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
		public decimal? AmountReceivedInPkr { get; set; }

		[StringLength(50)]
        public string? PaymentStatus { get; set; }
        [StringLength(100)]
        public string? SourceOfPayment { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [StringLength(1000)]
        public string? OrderDetails { get; set; }

        [StringLength(100)]
        public string? WordsOrPages { get; set; }

        public decimal? TotalPayment { get; set; }

        [StringLength(200)]
        public string? AssignedTo { get; set; }

        // Writer Portal fields
        [StringLength(2000)]
        public string? AttachmentFilenames { get; set; }

        [StringLength(500)]
        public string? SubmittedFilename { get; set; }

        [StringLength(50)]
        public string? SubmissionStatus { get; set; }

        public DateTime? SubmissionDate { get; set; }

        [StringLength(100)]
        public string? AcademicLevel { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        // Currency-related fields for Digital Marketing companies
        [StringLength(10)]
        public string? CurrencyCode { get; set; }

        public decimal? ForeignCurrencyAmount { get; set; }

        public decimal? PkrAmount { get; set; }

        public decimal? ExchangeRateUsed { get; set; }
        
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
        [StringLength(100)]
        public string? ServiceType { get; set; }

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
        [StringLength(100)]
        public string? ServiceType { get; set; }

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










    public class ServiceTypeDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class CreateServiceTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // Writer Portal specific DTOs
    public class WriterSubmissionDto
    {
        [Required]
        public int SaleOrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string SubmissionStatus { get; set; } = string.Empty; // "Completed" or "Need clarity"

        [StringLength(500)]
        public string? SubmittedFilename { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class WriterAssignmentDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string? ClientName { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Status { get; set; }
        public string? OrderDetails { get; set; }
        public string? WordsOrPages { get; set; }
        public string? AcademicLevel { get; set; }
        public string? AttachmentFilenames { get; set; }
        public string? SubmissionStatus { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string? SubmittedFilename { get; set; }
    }

    // Team Lead DTOs
    public class TeamLeadOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string? ClientName { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Status { get; set; }
        public string? AssignedTo { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? OrderDetails { get; set; }
        public string? AcademicLevel { get; set; }
        public string? Country { get; set; }
        public string? WordsOrPages { get; set; }
        public string? SubmissionStatus { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string? AttachmentFilenames { get; set; }
    }

    public class AssignOrderDto
    {
        public int SaleOrderId { get; set; }
        public int WriterId { get; set; }
    }

    // Order Instructions DTOs
    public class OrderInstructionDto
    {
        public int Id { get; set; }
        public int SaleOrderId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsFromTeamLead { get; set; }
        public string? AttachmentFilenames { get; set; }
    }

    public class CreateOrderInstructionDto
    {
        [Required]
        public int SaleOrderId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AttachmentFilenames { get; set; }
    }

    public class UpdateOrderDetailsDto
    {
        [Required]
        public int Id { get; set; }
        
        [StringLength(1000)]
        public string? OrderDetails { get; set; }
    }