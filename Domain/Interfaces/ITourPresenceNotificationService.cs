using System.Collections.Generic;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourPresenceNotificationService
    {
        List<TourPresenceNotificationDTO> GetAll();
        TourPresenceNotificationDTO GetById(int id);
        TourPresenceNotificationDTO Add(TourPresenceNotificationDTO notification);
        void Delete(TourPresenceNotificationDTO notification);
        TourPresenceNotificationDTO MarkAsRead(int id);
        List<TourPresenceNotificationDTO> GetUnreadByUserId(int userId);
        void CreatePresenceNotification(int tourId, int userId, string message);
    }
}