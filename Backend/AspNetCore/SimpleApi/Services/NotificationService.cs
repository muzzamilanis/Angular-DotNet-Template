using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.DTOs;
using SimpleApi.Models;

namespace SimpleApi.Services
{
    public interface INotificationService
    {
        Task<DashboardNotificationResponseDto> GetDashboardNotificationsAsync(int userId, int companyId);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISaleOrderService _saleOrderService;

        public NotificationService(ApplicationDbContext context, ISaleOrderService saleOrderService)
        {
            _context = context;
            _saleOrderService = saleOrderService;
        }

        public async Task<DashboardNotificationResponseDto> GetDashboardNotificationsAsync(int userId, int companyId)
        {
            // Get current user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new DashboardNotificationResponseDto();
            }

			// Get the current date in UTC for PostgreSQL consistency
			var currentDateUtc = DateTime.UtcNow.Date;
			var deadlineLimitUtc = currentDateUtc.AddDays(3);

			// Get orders with approaching deadlines
			var approachingOrders = await _context.SaleOrders
                .Where(so => so.CompanyId == companyId && 
                            so.Status == "InProgress" && 
                            so.Deadline.HasValue && 
                            so.Deadline.Value.ToUniversalTime() <= deadlineLimitUtc)
                .ToListAsync();

            var notifications = new List<NotificationDto>();

            foreach (var order in approachingOrders)
            {
                // Check if user should see this notification
                if (!ShouldUserSeeNotification(user, order))
                    continue;

                var daysUntilDeadline = order.Deadline.HasValue 
                    ? (order.Deadline.Value.Date - currentDateUtc).Days 
                    : 0;

                var notification = new NotificationDto
                {
                    SaleOrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    ClientName = order.ClientName ?? order.CustomerName,
                    Deadline = order.Deadline?.ToUniversalTime(),
                    DaysUntilDeadline = daysUntilDeadline,
                    AssignedTo = order.AssignedTo,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    UrgencyLevel = GetUrgencyLevel(daysUntilDeadline),
                    Message = CreateNotificationMessage(order, daysUntilDeadline)
                };

                notifications.Add(notification);
            }

            // Sort by urgency (most urgent first)
            notifications = notifications
                .OrderBy(n => GetUrgencyPriority(n.UrgencyLevel))
                .ThenBy(n => n.DaysUntilDeadline)
                .ToList();

            return new DashboardNotificationResponseDto
            {
                Notifications = notifications,
                TotalCount = notifications.Count,
                HasCriticalNotifications = notifications.Any(n => n.UrgencyLevel == "Critical" || n.UrgencyLevel == "High")
            };
        }

        private static bool ShouldUserSeeNotification(User user, SaleOrder order)
        {
            // SuperAdmins see all notifications
            if (user.IsSuperAdmin)
                return true;

            // Regular users (Writers) only see orders assigned to them
            if (!string.IsNullOrEmpty(order.AssignedTo))
            {
                return order.AssignedTo.Contains(user.Name, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static string GetUrgencyLevel(int daysUntilDeadline)
        {
            return daysUntilDeadline switch
            {
                <= 0 => "Critical",
                1 => "High",
                2 => "Medium",
                _ => "Low"
            };
        }

        private static int GetUrgencyPriority(string urgencyLevel)
        {
            return urgencyLevel switch
            {
                "Critical" => 1,
                "High" => 2,
                "Medium" => 3,
                "Low" => 4,
                _ => 5
            };
        }

        private static string CreateNotificationMessage(SaleOrder order, int daysUntilDeadline)
        {
            var clientName = order.ClientName ?? order.CustomerName ?? "Unknown Client";
            
            return daysUntilDeadline switch
            {
                < 0 => $"Order {order.OrderNumber} for {clientName} is OVERDUE by {Math.Abs(daysUntilDeadline)} day(s)!",
                0 => $"Order {order.OrderNumber} for {clientName} is due TODAY!",
                1 => $"Order {order.OrderNumber} for {clientName} is due TOMORROW!",
                _ => $"Order {order.OrderNumber} for {clientName} is due in {daysUntilDeadline} days."
            };
        }
    }
}