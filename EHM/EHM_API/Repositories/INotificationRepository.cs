using EHM_API.Models;

namespace EHM_API.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllNotificationsAsync();
        Task<Notification?> GetNotificationByIdAsync(int id);
        Task CreateNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int id);
    }
}
