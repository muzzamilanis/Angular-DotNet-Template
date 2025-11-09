namespace SimpleApi.DTOs
{
    public class PropertySaleDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string? BuyerAddress { get; set; }
        public string? BuyerPhone { get; set; }
        public string? BuyerEmail { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal ProfitOrLoss { get; set; }
        public decimal? AdvanceReceived { get; set; }
        public decimal? RemainingReceivable { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
        public string? Notes { get; set; }
        public int CompanyId { get; set; }
        public int? VoucherId { get; set; }
        public PropertyDto? Property { get; set; }
    }

    public class CreatePropertySaleDto
    {
        public int PropertyId { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string? BuyerAddress { get; set; }
        public string? BuyerPhone { get; set; }
        public string? BuyerEmail { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? AdvanceReceived { get; set; }
        public decimal? RemainingReceivable { get; set; }
        public string? PaymentStatus { get; set; } = "Pending";
        public string? PaymentMethod { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePropertySaleDto : CreatePropertySaleDto
    {
        public int Id { get; set; }
    }
}
