namespace SimpleApi.Services
{
    public interface INotificationSenderFactory
    {
        INotificationSender GetNotificationSender();
    }

    public class NotificationSenderFactory : INotificationSenderFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationSenderFactory> _logger;

        public NotificationSenderFactory(
            IConfiguration configuration, 
            IServiceProvider serviceProvider,
            ILogger<NotificationSenderFactory> logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public INotificationSender GetNotificationSender()
        {
            var notificationMode = _configuration["Notification:Mode"]?.ToLower() ?? "email";
            
            _logger.LogInformation("Using notification mode: {Mode}", notificationMode);

            return notificationMode switch
            {
                "whatsapp" => new WhatsAppNotificationSender(
                    _serviceProvider.GetRequiredService<IWhatsAppService>()),
                "email" => new EmailNotificationSender(
                    _serviceProvider.GetRequiredService<IEmailService>()),
                _ => new EmailNotificationSender(
                    _serviceProvider.GetRequiredService<IEmailService>())
            };
        }
    }
}
