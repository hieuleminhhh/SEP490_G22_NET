using EHM_API.DTOs.NotificationDTO;

namespace EHM_API.Services
{
    public interface INotificationService
    {
        Task<List<NotificationAllDTO>> GetAllNotificationsAsync();
        Task<List<NotificationAllDTO>> GetNotificationsByAccountIdAsync(int accountId);
        Task<List<NotificationAllDTO>> GetNotificationsByTypeAsync(int type);
        Task CreateNotificationAsync(NotificationCreateDTO notificationDto);
        Task DeleteNotificationAsync(int id);
    }
}
