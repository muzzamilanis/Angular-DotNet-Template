using SimpleApi.DTOs;
using System.Text.Json;

namespace SimpleApi.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateResponseDto> GetExchangeRateAsync(string currencyCode);
    }

    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ExchangeRateResponseDto> GetExchangeRateAsync(string currencyCode)
        {
            try
            {
                // Using ExchangeRate-API (free tier) - base currency PKR
                var response = await _httpClient.GetAsync($"https://api.exchangerate-api.com/v4/latest/PKR");
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch exchange rate. Status: {response.StatusCode}");
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var exchangeData = JsonSerializer.Deserialize<ExchangeRateApiResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (exchangeData?.Rates == null || !exchangeData.Rates.ContainsKey(currencyCode.ToUpper()))
                {
                    throw new ArgumentException($"Currency code '{currencyCode}' not found");
                }

                var rate = exchangeData.Rates[currencyCode.ToUpper()];

                return new ExchangeRateResponseDto
                {
                    CurrencyCode = currencyCode.ToUpper(),
                    ExchangeRate = rate,
                    BaseCurrency = "PKR",
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rate for currency: {CurrencyCode}", currencyCode);
                throw;
            }
        }
    }

    // Internal class for deserializing the API response
    internal class ExchangeRateApiResponse
    {
        public string Base { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new();
        public DateTime Date { get; set; }
    }
}