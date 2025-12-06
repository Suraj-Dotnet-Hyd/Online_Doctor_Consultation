using NotificationService.Models;

namespace NotificationService.Service
{
    public interface INotificationService
    {
        Task<bool> SendEmailAsync(NotificationRequest request);
    }
}
