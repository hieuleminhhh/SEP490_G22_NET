using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllNotificationsAsync();
        Task<List<Notification>> GetNotificationsByAccountIdAsync(int accountId);
        Task<List<Notification>> GetNotificationsByTypeAsync(int type);
        Task CreateNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int id);
        Task<bool> UpdateIsViewAsync(int notificationId);
    }
}
