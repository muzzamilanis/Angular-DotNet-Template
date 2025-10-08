namespace SimpleApi.Services
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly IEmailService _emailService;

        public EmailNotificationSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendNotificationAsync(string recipient, string subject, string body)
        {
            await _emailService.SendEmailAsync(recipient, subject, body);
        }

        public async Task SendNotificationAsync(List<string> recipients, string subject, string body)
        {
            await _emailService.SendEmailAsync(recipients, subject, body);
        }

        public string FormatDeadlineNotification(Models.SaleOrder order)
        {
            var daysUntilDeadline = order.Deadline.HasValue 
                ? (order.Deadline.Value.Date - DateTime.Today).Days 
                : 0;

            var urgencyMessage = daysUntilDeadline switch
            {
                0 => "due TODAY",
                1 => "due TOMORROW", 
                _ => $"due in {daysUntilDeadline} days"
            };

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #d9534f;'>⚠️ Deadline Reminder</h2>
                    <p>This is a reminder that the following sale order is <strong>{urgencyMessage}</strong>:</p>
                    
                    <div style='background-color: #f9f9f9; padding: 15px; border-left: 4px solid #d9534f; margin: 10px 0;'>
                        <p><strong>Order Number:</strong> {order.OrderNumber}</p>
                        <p><strong>Client Name:</strong> {order.ClientName ?? order.CustomerName ?? "N/A"}</p>
                        <p><strong>Deadline:</strong> {order.Deadline?.ToString("MMM dd, yyyy") ?? "N/A"}</p>
                        <p><strong>Status:</strong> {order.Status}</p>
                        <p><strong>Assigned To:</strong> {order.AssignedTo ?? "Unassigned"}</p>
                        {(order.TotalAmount > 0 ? $"<p><strong>Total Amount:</strong> ${order.TotalAmount:F2}</p>" : "")}
                    </div>
                    
                    <p>Please take appropriate action to ensure timely delivery.</p>
                    
                    <hr style='margin: 20px 0;'/>
                    <p style='font-size: 12px; color: #666;'>
                        This is an automated notification from the Sale Order Management System.
                        <br/>You will continue to receive these notifications until the order status changes to 'Delivered'.
                    </p>
                </body>
                </html>";
        }
    }
}
