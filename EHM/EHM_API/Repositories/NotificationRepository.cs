using EHM_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly EHMDBContext _context;

        public NotificationRepository(EHMDBContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications
                .Include(n => n.Order)  
                .Include(o => o.Account)  
                .Where(n => n.Account.Role != "User")  
                .OrderByDescending(n => n.Time) 
                .ToListAsync();
        }



        public async Task<List<Notification>> GetNotificationsByAccountIdAsync(int accountId)
        {
            return await _context.Notifications
                .Where(n => n.AccountId == accountId)
                .OrderByDescending(n => n.Time)
                .ToListAsync();
        }
        public async Task<List<Notification>> GetNotificationsByTypeAsync(int type)
        {
            return await _context.Notifications
                .Where(n => n.Type == type)
                .OrderByDescending(n => n.Time)
                .ToListAsync();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            notification.IsView = false;
            notification.Time = DateTime.Now;
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> UpdateIsViewAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if (notification == null)
            {
                return false; // Không tìm thấy Notification
            }

            // Cập nhật IsView
            notification.IsView = true;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return true; // Cập nhật thành công
        }
    }
}
