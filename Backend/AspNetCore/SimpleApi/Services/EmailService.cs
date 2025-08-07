using System.Net;
using System.Net.Mail;

namespace SimpleApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(new List<string> { to }, subject, body);
        }

        public async Task SendEmailAsync(List<string> recipients, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "localhost";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:Username"] ?? "";
                var smtpPassword = _configuration["Email:Password"] ?? "";
                var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@company.com";
                var fromName = _configuration["Email:FromName"] ?? "Sale Order System";

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                foreach (var recipient in recipients)
                {
                    if (!string.IsNullOrEmpty(recipient))
                    {
                        mailMessage.To.Add(recipient);
                    }
                }

                if (mailMessage.To.Count > 0)
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", recipients));
                }
                else
                {
                    _logger.LogWarning("No valid recipients found for email: {Subject}", subject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipients}. Subject: {Subject}", 
                    string.Join(", ", recipients), subject);
                // Don't throw to prevent background service from failing
            }
        }
    }
}