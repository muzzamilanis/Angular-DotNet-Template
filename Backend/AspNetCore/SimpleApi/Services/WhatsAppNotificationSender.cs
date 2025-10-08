namespace SimpleApi.Services
{
    public class WhatsAppNotificationSender : INotificationSender
    {
        private readonly IWhatsAppService _whatsAppService;

        public WhatsAppNotificationSender(IWhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        public async Task SendNotificationAsync(string recipient, string subject, string body)
        {
            // For WhatsApp, we ignore the subject and send only the body
            await _whatsAppService.SendWhatsAppAsync(recipient, body);
        }

        public async Task SendNotificationAsync(List<string> recipients, string subject, string body)
        {
            // For WhatsApp, we ignore the subject and send only the body
            await _whatsAppService.SendWhatsAppAsync(recipients, body);
        }

        public string FormatDeadlineNotification(Models.SaleOrder order)
        {
            var daysUntilDeadline = order.Deadline.HasValue 
                ? (order.Deadline.Value.Date - DateTime.Today).Days 
                : 0;

            // Create a WhatsApp-friendly message with emojis
            var urgencyEmoji = daysUntilDeadline switch
            {
                0 => "🚨",
                1 => "⚠️",
                2 => "⏰",
                _ => "📌"
            };

            var urgencyText = daysUntilDeadline switch
            {
                0 => "*DUE TODAY*",
                1 => "*DUE TOMORROW*",
                _ => $"*Due in {daysUntilDeadline} days*"
            };

            // Format message in WhatsApp style - concise and using emojis
            var message = $@"{urgencyEmoji} *Deadline Reminder*

{urgencyText}

📋 *Order:* {order.OrderNumber}
👤 *Client:* {order.ClientName ?? order.CustomerName ?? "N/A"}
📅 *Deadline:* {order.Deadline?.ToString("MMM dd, yyyy") ?? "N/A"}
📊 *Status:* {order.Status}
✍️ *Assigned:* {order.AssignedTo ?? "Unassigned"}";

            if (order.TotalAmount > 0)
            {
                message += $"\n💰 *Amount:* ${order.TotalAmount:F2}";
            }

            message += "\n\n⚡ Please take action to ensure timely delivery!";

            return message;
        }
    }
}
