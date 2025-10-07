using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ForumNotificationRepository : IForumNotificationRepository
    {
        private const string FilePath = "../../../Resources/Data/forumNotifications.csv";
        private readonly Serializer<ForumNotification> _serializer;
        private List<ForumNotification> _notifications;

        public ForumNotificationRepository()
        {
            _serializer = new Serializer<ForumNotification>();
            _notifications = _serializer.FromCSV(FilePath);
        }

        private void SaveToFile()
        {
            _serializer.ToCSV(FilePath, _notifications);
        }

        public ForumNotification Save(ForumNotification notification)
        {
            notification.Id = NextId();
            _notifications.Add(notification);
            SaveToFile();
            return notification;
        }

        public List<ForumNotification> GetAll()
        {
            return _notifications.ToList();
        }

        public List<ForumNotification> GetByOwnerId(int ownerId)
        {
            return _notifications.Where(n => n.OwnerId == ownerId).ToList();
        }

        public List<ForumNotification> GetUnreadByOwnerId(int ownerId)
        {
            return _notifications.Where(n => n.OwnerId == ownerId && !n.IsRead).ToList();
        }

        public ForumNotification GetById(int id)
        {
            return _notifications.FirstOrDefault(n => n.Id == id);
        }

        public ForumNotification Update(ForumNotification notification)
        {
            var existing = _notifications.FirstOrDefault(n => n.Id == notification.Id);
            if (existing != null)
            {
                existing.IsRead = notification.IsRead;
                SaveToFile();
            }
            return existing;
        }

        private int NextId()
        {
            return _notifications.Count == 0 ? 1 : _notifications.Max(n => n.Id) + 1;
        }
    }
}