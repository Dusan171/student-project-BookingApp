using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TourNotificationRepository : ITourNotificationRepository
    {
        private const string FilePath = "../../../Resources/Data/tour_notifications.csv";
        private readonly Serializer<TourNotification> _serializer;
        private List<TourNotification> _notifications;

        public TourNotificationRepository()
        {
            _serializer = new Serializer<TourNotification>();
            _notifications = _serializer.FromCSV(FilePath);
        }

        public TourNotification GetById(int id)
        {
            return _notifications.FirstOrDefault(n => n.Id == id);
        }

        public List<TourNotification> GetAll()
        {
            return _notifications.ToList();
        }

        public TourNotification Save(TourNotification notification)
        {
            notification.Id = NextId();
            notification.CreatedAt = DateTime.Now;
            _notifications.Add(notification);
            _serializer.ToCSV(FilePath, _notifications);
            return notification;
        }

        public TourNotification Update(TourNotification notification)
        {
            var existing = GetById(notification.Id);
            if (existing != null)
            {
                existing.TouristId = notification.TouristId;
                existing.TourId = notification.TourId;
                existing.Title = notification.Title;
                existing.Message = notification.Message;
                existing.CreatedAt = notification.CreatedAt;
                existing.IsRead = notification.IsRead;

                _serializer.ToCSV(FilePath, _notifications);
                return existing;
            }
            return null;
        }

        public void Delete(TourNotification notification)
        {
            _notifications.RemoveAll(n => n.Id == notification.Id);
            _serializer.ToCSV(FilePath, _notifications);
        }

        public List<TourNotification> GetByTouristId(int touristId)
        {
            return _notifications
                .Where(n => n.TouristId == touristId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public List<TourNotification> GetUnreadByTouristId(int touristId)
        {
            return _notifications
                .Where(n => n.TouristId == touristId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        private int NextId()
        {
            return _notifications.Count > 0 ? _notifications.Max(n => n.Id) + 1 : 1;
        }
    }
}