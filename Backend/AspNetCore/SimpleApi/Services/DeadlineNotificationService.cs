using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.Services;

namespace SimpleApi.Services
{
    public class DeadlineNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeadlineNotificationService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(24); // Run once per day

        public DeadlineNotificationService(
            IServiceProvider serviceProvider,
            ILogger<DeadlineNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_period, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                await ProcessDeadlineNotificationsAsync();
            }
        }

        private async Task ProcessDeadlineNotificationsAsync()
        {
            try
            {
                _logger.LogInformation("Starting deadline notification check at {Time}", DateTime.UtcNow);

                using var scope = _serviceProvider.CreateScope();
                var saleOrderService = scope.ServiceProvider.GetRequiredService<ISaleOrderService>();
                var notificationFactory = scope.ServiceProvider.GetRequiredService<INotificationSenderFactory>();
                var notificationSender = notificationFactory.GetNotificationSender();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var approachingOrders = await saleOrderService.GetDeadlineApproachingOrdersAsync();
                
                if (!approachingOrders.Any())
                {
                    _logger.LogInformation("No orders with approaching deadlines found");
                    return;
                }

                _logger.LogInformation("Found {Count} orders with approaching deadlines", approachingOrders.Count);

                // Get all super admins
                var superAdmins = await context.Users
                    .Where(u => u.IsSuperAdmin && u.IsActive)
                    .Select(u => u.Email)
                    .ToListAsync();

                foreach (var order in approachingOrders)
                {
                    var recipients = new List<string>();
                    
                    // Add super admins
                    recipients.AddRange(superAdmins);
                    
                    // Add assigned writer if specified
                    if (!string.IsNullOrEmpty(order.AssignedTo))
                    {
                        // Try to find user by name (AssignedTo contains the name)
                        var assignedWriter = await context.Users
                            .Where(u => u.Name.Contains(order.AssignedTo) && u.IsActive && u.CompanyId == order.CompanyId)
                            .Select(u => u.Email)
                            .FirstOrDefaultAsync();
                        
                        if (!string.IsNullOrEmpty(assignedWriter) && !recipients.Contains(assignedWriter))
                        {
                            recipients.Add(assignedWriter);
                        }
                    }

                    if (recipients.Any())
                    {
                        var subject = $"Deadline Reminder: Sale Order {order.OrderNumber}";
                        var body = notificationSender.FormatDeadlineNotification(order);
                        
                        await notificationSender.SendNotificationAsync(recipients, subject, body);
                        _logger.LogInformation("Sent deadline notification for order {OrderNumber} to {RecipientCount} recipients",
                            order.OrderNumber, recipients.Count);
                    }
                }

                _logger.LogInformation("Completed deadline notification check at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing deadline notifications");
            }
        }

        private static string CreateEmailBody(Models.SaleOrder order)
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