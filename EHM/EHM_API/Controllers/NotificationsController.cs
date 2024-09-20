using EHM_API.DTOs;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using EHM_API.DTOs.NotificationDTO;
using EHM_API.Repositories;

namespace EHM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(INotificationService notificationService, INotificationRepository notificationRepository)
        {
            _notificationService = notificationService;
            _notificationRepository = notificationRepository;
        }



        // GET: api/Notification
        [HttpGet] 
        public async Task<ActionResult<List<NotificationAllDTO>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        // GET: api/Notification/account/{accountId}
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<List<NotificationAllDTO>>> GetNotificationsByAccountId(int accountId)
        {
            var notifications = await _notificationService.GetNotificationsByAccountIdAsync(accountId);
            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found for this account.");
            }
            return Ok(notifications);
        }
        [HttpGet("type/{type}")]
        public async Task<ActionResult<List<NotificationAllDTO>>> GetNotificationsByType(int type)
        {
            var notifications = await _notificationService.GetNotificationsByTypeAsync(type);
            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found for this account.");
            }
            return Ok(notifications);
        }
        [HttpPost]
        public async Task<ActionResult> CreateNotification([FromBody] NotificationCreateDTO notificationDto)
        {
            await _notificationService.CreateNotificationAsync(notificationDto);
            return Ok(new { message = "Notification created successfully" });
        }

    
        // DELETE: api/Notification/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }

        [HttpPut("UpdateIsView/{notificationId}")]
        public async Task<IActionResult> UpdateIsView(int notificationId)
        {
            var result = await _notificationRepository.UpdateIsViewAsync(notificationId);

            if (!result)
            {
                return NotFound(new { Message = "Notification not found." });
            }

            return Ok(new { Message = "Notification updated successfully." });
        }
    }
}
