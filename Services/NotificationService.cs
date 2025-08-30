using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IReservationService _reservationService;
        public NotificationService(INotificationRepository repository, IReservationService reservationService)
        {
            _repository = repository;
            _reservationService = reservationService;

        }
        public List<NotificationDTO> GetAll()
        {
            return _repository.GetAll()
                              .ConvertAll(n => new NotificationDTO(n));
        }
        public NotificationDTO GetById(int id)
        {
            var notification = _repository.GetById(id);
            return _repository.GetById(id) == null ? null : new NotificationDTO(notification);
        }
        public NotificationDTO Add(NotificationDTO notification)
        {
            var entity = notification.ToNotification();
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
            var updated = _repository.Update(notification);
            return new NotificationDTO(updated);
        }
        public void CheckAndGenerateNotifications()
        {
            var unratedReservations = _reservationService.GetUnratedReservations();
            var allNotifications = _repository.GetAll();
            var expiredNotifications = allNotifications.Where(n => (DateTime.Now - n.Deadline).TotalDays > 0).ToList();
            foreach (var notification in expiredNotifications)
            { _repository.Delete(notification);}
            foreach (var reservation in unratedReservations)
            {
                var notificationExists = _repository.GetAll().Any(n => n.ReservationId == reservation.Id);

                if (!notificationExists)
                {
                    var newNotification = new Notification { ReservationId = reservation.Id,GuestId = reservation.GuestId,Deadline = reservation.EndDate.AddDays(5),IsRead = false};
                    _repository.Save(newNotification);
                }
            }
        }
    }
}

