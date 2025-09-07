using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;

namespace BookingApp.Services
{
    public class TourPresenceNotificationService : ITourPresenceNotificationService
    {
        private readonly ITourPresenceNotificationRepository _repository;

        public TourPresenceNotificationService(ITourPresenceNotificationRepository repository)
        {
            _repository = repository;
        }

        public List<TourPresenceNotificationDTO> GetAll()
        {
            return _repository.GetAll()
                             .ConvertAll(n => new TourPresenceNotificationDTO(n));
        }

        public TourPresenceNotificationDTO GetById(int id)
        {
            var notification = _repository.GetById(id);
            return notification == null ? null : new TourPresenceNotificationDTO(notification);
        }

        public TourPresenceNotificationDTO Add(TourPresenceNotificationDTO notification)
        {
            var entity = notification.ToTourPresenceNotification();
            var saved = _repository.Save(entity);
            return new TourPresenceNotificationDTO(saved);
        }

        public void Delete(TourPresenceNotificationDTO notification)
        {
            _repository.Delete(notification.ToTourPresenceNotification());
        }

        public TourPresenceNotificationDTO MarkAsRead(int id)
        {
            var notification = _repository.GetById(id);
            if (notification == null) return null;

            notification.IsRead = true;
            var updated = _repository.Update(notification);
            return new TourPresenceNotificationDTO(updated);
        }

        public List<TourPresenceNotificationDTO> GetUnreadByUserId(int userId)
        {
            return _repository.GetUnreadByUserId(userId)
                             .ConvertAll(n => new TourPresenceNotificationDTO(n));
        }

        public void CreatePresenceNotification(int tourId, int userId, string message)
        {
            var notification = new TourPresenceNotificationDTO
            {
                TourId = tourId,
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            Add(notification);
        }
    }
}