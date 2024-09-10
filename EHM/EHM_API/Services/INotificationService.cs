using EHM_API.DTOs.NotificationDTO;

namespace EHM_API.Services
{
    public interface INotificationService
    {
        Task<List<NotificationAllDTO>> GetAllNotificationsAsync();
        Task<NotificationAllDTO?> GetNotificationByIdAsync(int id);
        Task CreateNotificationAsync(NotificationCreateDTO notificationDto);
        Task UpdateNotificationAsync(int id, NotificationCreateDTO notificationDto);
        Task DeleteNotificationAsync(int id);
    }
}
