using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourNotificationService
    {
        List<TourNotificationDTO> GetNotificationsByTourist(int touristId);
        TourNotificationDTO GetLatestNotification(int touristId);
        void CreateNotificationsForNewTour(int tourId, string tourName, string location, string language);
        void SendTourAcceptanceNotification(int touristId, int tourId, string tourName, string location, DateTime scheduledDate, string guideName);
        void MarkAsRead(int notificationId);
    }
}