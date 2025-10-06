using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourNotificationService
    {
        List<TourNotificationDTO> GetNotificationsByTourist(int touristId);
        TourNotificationDTO GetLatestNotification(int touristId);
        void CreateNotificationsForNewTour(int tourId, string tourName, string location, string language);
        void MarkAsRead(int notificationId);
    }
}