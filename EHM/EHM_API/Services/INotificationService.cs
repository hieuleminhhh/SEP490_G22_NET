using EHM_API.DTOs.NotificationDTO;

namespace EHM_API.Services
{
    public interface INotificationService
    {
        Task<List<NotificationAllDTO>> GetAllNotificationsAsync();
        Task<List<NotificationAllDTO>> GetNotificationsByAccountIdAsync(int accountId);
        Task CreateNotificationAsync(NotificationCreateDTO notificationDto);
        Task DeleteNotificationAsync(int id);
    }
}
