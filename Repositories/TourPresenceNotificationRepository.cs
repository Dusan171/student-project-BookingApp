using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;

namespace BookingApp.Repositories
{
    public class TourPresenceNotificationRepository : ITourPresenceNotificationRepository
    {
        private const string FilePath = "../../../Resources/Data/tourPresenceNotifications.csv";
        private readonly Serializer<TourPresenceNotification> _serializer;
        private List<TourPresenceNotification> _notifications;

        public TourPresenceNotificationRepository()
        {
            _serializer = new Serializer<TourPresenceNotification>();
            _notifications = _serializer.FromCSV(FilePath) ?? new List<TourPresenceNotification>();
        }

        public List<TourPresenceNotification> GetAll()
        {
            return _serializer.FromCSV(FilePath) ?? new List<TourPresenceNotification>();
        }

        public TourPresenceNotification GetById(int id)
        {
            return _notifications.FirstOrDefault(n => n.Id == id);
        }

        public TourPresenceNotification Save(TourPresenceNotification notification)
        {
            notification.Id = NextId();
            _notifications.Add(notification);
            _serializer.ToCSV(FilePath, _notifications);
            return notification;
        }

        public void Delete(TourPresenceNotification notification)
        {
            var found = _notifications.Find(n => n.Id == notification.Id);
            if (found != null)
            {
                _notifications.Remove(found);
                _serializer.ToCSV(FilePath, _notifications);
            }
        }

        public TourPresenceNotification Update(TourPresenceNotification notification)
        {
            var current = _notifications.Find(n => n.Id == notification.Id);
            if (current != null)
            {
                int index = _notifications.IndexOf(current);
                _notifications.Remove(current);
                _notifications.Insert(index, notification);
                _serializer.ToCSV(FilePath, _notifications);
            }
            return notification;
        }

        public List<TourPresenceNotification> GetByUserId(int userId)
        {
            return _notifications.Where(n => n.UserId == userId).ToList();
        }

        public List<TourPresenceNotification> GetUnreadByUserId(int userId)
        {
            return _notifications.Where(n => n.UserId == userId && !n.IsRead).ToList();
        }

        public int NextId()
        {
            _notifications = _serializer.FromCSV(FilePath) ?? new List<TourPresenceNotification>();
            if (_notifications.Count < 1)
            {
                return 1;
            }
            return _notifications.Max(n => n.Id) + 1;
        }
    }
}