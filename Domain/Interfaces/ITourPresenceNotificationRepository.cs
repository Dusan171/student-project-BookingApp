using System.Collections.Generic;
using BookingApp.Domain.Model;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourPresenceNotificationRepository
    {
        List<TourPresenceNotification> GetAll();
        TourPresenceNotification GetById(int id);
        TourPresenceNotification Save(TourPresenceNotification notification);
        void Delete(TourPresenceNotification notification);
        TourPresenceNotification Update(TourPresenceNotification notification);
        List<TourPresenceNotification> GetByUserId(int userId);
        List<TourPresenceNotification> GetUnreadByUserId(int userId);
        int NextId();
    }
}