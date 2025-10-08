using System.Net.Http;
using System.Text;

namespace SimpleApi.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly HttpClient _httpClient;

        public WhatsAppService(IConfiguration configuration, ILogger<WhatsAppService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task SendWhatsAppAsync(string to, string message)
        {
            await SendWhatsAppAsync(new List<string> { to }, message);
        }

        public async Task SendWhatsAppAsync(List<string> recipients, string message)
        {
            try
            {
                var accountSid = _configuration["WhatsApp:AccountSid"];
                var authToken = _configuration["WhatsApp:AuthToken"];
                var fromNumber = _configuration["WhatsApp:FromNumber"];

                // Validate WhatsApp configuration
                if (string.IsNullOrEmpty(accountSid))
                {
                    _logger.LogError("WhatsApp configuration error: AccountSid is not configured");
                    return;
                }

                if (string.IsNullOrEmpty(authToken))
                {
                    _logger.LogError("WhatsApp configuration error: AuthToken is not configured");
                    return;
                }

                if (string.IsNullOrEmpty(fromNumber))
                {
                    _logger.LogError("WhatsApp configuration error: FromNumber is not configured");
                    return;
                }

                foreach (var recipient in recipients)
                {
                    if (string.IsNullOrEmpty(recipient))
                        continue;

                    await SendSingleWhatsAppMessageAsync(accountSid, authToken, fromNumber, recipient, message);
                }

                _logger.LogInformation("WhatsApp messages sent successfully to {RecipientCount} recipients", recipients.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WhatsApp messages to {Recipients}", 
                    string.Join(", ", recipients));
                // Don't throw to prevent background service from failing
            }
        }

        private async Task SendSingleWhatsAppMessageAsync(string accountSid, string authToken, string fromNumber, string toNumber, string message)
        {
            try
            {
                // Ensure phone numbers are in WhatsApp format (whatsapp:+1234567890)
                var formattedFrom = fromNumber.StartsWith("whatsapp:") ? fromNumber : $"whatsapp:{fromNumber}";
                var formattedTo = toNumber.StartsWith("whatsapp:") ? toNumber : $"whatsapp:{toNumber}";

                // Twilio API endpoint
                var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json";

                // Create form data
                var formData = new Dictionary<string, string>
                {
                    { "From", formattedFrom },
                    { "To", formattedTo },
                    { "Body", message }
                };

                var content = new FormUrlEncodedContent(formData);

                // Add basic authentication
                var authBytes = Encoding.ASCII.GetBytes($"{accountSid}:{authToken}");
                var authHeader = Convert.ToBase64String(authBytes);
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("WhatsApp message sent successfully to {Recipient}", toNumber);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send WhatsApp message to {Recipient}. Status: {Status}, Error: {Error}", 
                        toNumber, response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp message to {Recipient}", toNumber);
            }
        }
    }
}
