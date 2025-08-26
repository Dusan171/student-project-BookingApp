using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public List<NotificationDTO> GetAll()
        {
            return _repository.GetAll()
                              .ConvertAll(n => new NotificationDTO(n));
        }

        public NotificationDTO GetById(int id)
        {
            var notification = _repository.GetById(id);
            return notification == null ? null : new NotificationDTO(notification);
        }

        public NotificationDTO Add(NotificationDTO notification)
        {
            var entity = notification.ToNotification();
            entity.Id = _repository.NextId();
            var saved = _repository.Save(entity);
            return new NotificationDTO(saved);
        }

        public void Delete(NotificationDTO notification)
        {
            _repository.Delete(notification.ToNotification());
        }

        public NotificationDTO MarkAsRead(int id)
        {
            var notification = _repository.GetById(id);
            if (notification == null) return null;

            notification.IsRead = true;
            var updated = _repository.Save(notification);
            return new NotificationDTO(updated);
        }

   
    }
}

