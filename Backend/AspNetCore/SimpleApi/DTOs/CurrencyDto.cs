using System.ComponentModel.DataAnnotations;

namespace SimpleApi.DTOs
{
    public class CurrencyDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CompanyId { get; set; }
    }

    public class CreateCurrencyDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;
    }

    public class UpdateCurrencyDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class ExchangeRateResponseDto
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal ExchangeRate { get; set; }
        public string BaseCurrency { get; set; } = "PKR";
        public DateTime LastUpdated { get; set; }
    }
}