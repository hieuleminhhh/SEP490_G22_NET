using AutoMapper;
using EHM_API.DTOs;
using EHM_API.DTOs.NotificationDTO;
using EHM_API.Models;
using EHM_API.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHM_API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<NotificationAllDTO>> GetAllNotificationsAsync()
        {
            var notifications = await _repository.GetAllNotificationsAsync();
            return _mapper.Map<List<NotificationAllDTO>>(notifications);
        }

        public async Task<NotificationAllDTO?> GetNotificationByIdAsync(int id)
        {
            var notification = await _repository.GetNotificationByIdAsync(id);
            return _mapper.Map<NotificationAllDTO>(notification);
        }

        public async Task CreateNotificationAsync(NotificationCreateDTO notificationDto)
        {
            var notification = _mapper.Map<Notification>(notificationDto);
            await _repository.CreateNotificationAsync(notification);
        }

        public async Task UpdateNotificationAsync(int id, NotificationCreateDTO notificationDto)
        {
            var notification = await _repository.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                throw new Exception("Notification not found");
            }

            _mapper.Map(notificationDto, notification);
            await _repository.UpdateNotificationAsync(notification);
        }

        public async Task DeleteNotificationAsync(int id)
        {
            await _repository.DeleteNotificationAsync(id);
        }
    }
}
