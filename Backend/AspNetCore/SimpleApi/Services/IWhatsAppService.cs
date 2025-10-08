namespace SimpleApi.Services
{
    public interface IWhatsAppService
    {
        Task SendWhatsAppAsync(string to, string message);
        Task SendWhatsAppAsync(List<string> recipients, string message);
    }
}
