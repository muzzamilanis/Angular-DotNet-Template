namespace SimpleApi.DTOs
{
    public class PropertyPurchaseDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public string? SellerAddress { get; set; }
        public string? SellerPhone { get; set; }
        public string? SellerEmail { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? AdvancePayment { get; set; }
        public decimal? RemainingPayment { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
        public string? Notes { get; set; }
        public int CompanyId { get; set; }
        public int? VoucherId { get; set; }
        public PropertyDto? Property { get; set; }
    }

    public class CreatePropertyPurchaseDto
    {
        public int PropertyId { get; set; }
        public string PurchaseNumber { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public string? SellerAddress { get; set; }
        public string? SellerPhone { get; set; }
        public string? SellerEmail { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? AdvancePayment { get; set; }
        public decimal? RemainingPayment { get; set; }
        public string? PaymentStatus { get; set; } = "Pending";
        public string? PaymentMethod { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePropertyPurchaseDto : CreatePropertyPurchaseDto
    {
        public int Id { get; set; }
    }
}
