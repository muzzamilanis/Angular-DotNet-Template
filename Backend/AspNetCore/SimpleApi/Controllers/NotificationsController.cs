using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.DTOs;
using SimpleApi.Services;

namespace SimpleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ISaleOrderService _saleOrderService;
        private readonly INotificationSenderFactory _notificationSenderFactory;

        public NotificationsController(
            INotificationService notificationService,
            IEmailService emailService,
            ISaleOrderService saleOrderService,
            INotificationSenderFactory notificationSenderFactory)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _saleOrderService = saleOrderService;
            _notificationSenderFactory = notificationSenderFactory;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardNotificationResponseDto>> GetDashboardNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var companyId = GetCurrentUserCompanyId();
                
                var result = await _notificationService.GetDashboardNotificationsAsync(userId, companyId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("send-deadline-notifications")]
        public async Task<ActionResult> SendDeadlineNotifications()
        {
            try
            {
                // Only super admins can manually trigger notifications
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only super admins can trigger notifications manually");
                }

                var companyId = GetCurrentUserCompanyId();
                var approachingOrders = await _saleOrderService.GetDeadlineApproachingOrdersAsync();
                var notificationSender = _notificationSenderFactory.GetNotificationSender();
                
                if (!approachingOrders.Any())
                {
                    return Ok(new { message = "No orders with approaching deadlines found", count = 0 });
                }

                var notificationsSent = 0;
                foreach (var order in approachingOrders.Where(o => o.CompanyId == companyId))
                {
                    var recipients = new List<string>();
                    
                    // Add the current user's email for testing
                    var currentUserEmail = GetCurrentUserEmail();
                    if (!string.IsNullOrEmpty(currentUserEmail))
                    {
                        recipients.Add(currentUserEmail);
                    }

                    if (recipients.Any())
                    {
                        var subject = $"Deadline Reminder: Sale Order {order.OrderNumber}";
                        var body = notificationSender.FormatDeadlineNotification(order);
                        
                        await notificationSender.SendNotificationAsync(recipients, subject, body);
                        notificationsSent++;
                    }
                }

                return Ok(new { 
                    message = "Deadline notifications sent successfully", 
                    ordersFound = approachingOrders.Count(o => o.CompanyId == companyId), 
                    notificationsSent = notificationsSent 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to send notifications", error = ex.Message });
            }
        }

        [HttpPost("test-email")]
        public async Task<ActionResult> TestEmail([FromBody] TestEmailDto testEmail)
        {
            try
            {
                // Only super admins can test email functionality
                if (!GetCurrentUserIsSuperAdmin())
                {
                    return Forbid("Only super admins can test email functionality");
                }

                var subject = testEmail.Subject ?? "Test Email from Sale Order System";
                var body = testEmail.Body ?? @"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #4CAF50;'>✅ Email Configuration Test</h2>
                        <p>This is a test email to verify that the email configuration is working correctly.</p>
                        <p><strong>Sent at:</strong> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
                        <p>If you receive this email, the notification system is functioning properly.</p>
                    </body>
                    </html>";

                var recipient = testEmail.ToEmail ?? GetCurrentUserEmail();
                if (string.IsNullOrEmpty(recipient))
                {
                    return BadRequest(new { message = "No recipient email address provided" });
                }

                await _emailService.SendEmailAsync(recipient, subject, body);
                
                return Ok(new { 
                    message = "Test email sent successfully", 
                    sentTo = recipient,
                    sentAt = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to send test email", error = ex.Message });
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