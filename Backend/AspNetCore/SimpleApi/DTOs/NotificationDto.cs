namespace SimpleApi.DTOs
{
    public class NotificationDto
    {
        public int SaleOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string? ClientName { get; set; }
        public DateTime? Deadline { get; set; }
        public int DaysUntilDeadline { get; set; }
        public string? AssignedTo { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public string UrgencyLevel { get; set; } = "Low"; // Low, Medium, High, Critical
        public string Message { get; set; } = string.Empty;
    }

    public class DashboardNotificationResponseDto
    {
        public List<NotificationDto> Notifications { get; set; } = new List<NotificationDto>();
        public int TotalCount { get; set; }
        public bool HasCriticalNotifications { get; set; }
    }
}