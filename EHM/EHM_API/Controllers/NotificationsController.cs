using EHM_API.DTOs;
using EHM_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using EHM_API.DTOs.NotificationDTO;

namespace EHM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notification
        [HttpGet] 
        public async Task<ActionResult<List<NotificationAllDTO>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        // GET: api/Notification/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationAllDTO>> GetNotificationById(int id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }
        [HttpPost]
        public async Task<ActionResult> CreateNotification([FromBody] NotificationCreateDTO notificationDto)
        {
            await _notificationService.CreateNotificationAsync(notificationDto);
            return Ok(new { message = "Notification created successfully" });
        }

        // PUT: api/Notification/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateNotification(int id, [FromBody] NotificationCreateDTO notificationDto)
        {
            await _notificationService.UpdateNotificationAsync(id, notificationDto);
            return Ok(new { message = "Notification updated successfully" });
        }

        // DELETE: api/Notification/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }
    }
}
