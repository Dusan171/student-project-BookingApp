using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourNotificationRepository
    {
        TourNotification GetById(int id);
        List<TourNotification> GetAll();
        TourNotification Save(TourNotification notification);
        TourNotification Update(TourNotification notification);
        void Delete(TourNotification notification);
        List<TourNotification> GetByTouristId(int touristId);
        List<TourNotification> GetUnreadByTouristId(int touristId);
    }
}