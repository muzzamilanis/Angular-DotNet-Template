namespace SimpleApi.Services
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(string recipient, string subject, string body);
        Task SendNotificationAsync(List<string> recipients, string subject, string body);
        string FormatDeadlineNotification(Models.SaleOrder order);
    }
}
