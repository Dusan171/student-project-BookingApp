using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;


namespace BookingApp.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

            private const string FilePath = "../../../Resources/Data/notifications.csv";

            private readonly Serializer<Notification> _serializer;

            private List<Notification> _notifications;

            public NotificationRepository()
            {
                _serializer = new Serializer<Notification>();
                _notifications = _serializer.FromCSV(FilePath) ?? new List<Notification>();
        }

            public List<Notification> GetAll()
            {
                return _serializer.FromCSV(FilePath);
            }

            public Notification GetById(int id)
            {
                return _notifications.FirstOrDefault(a => a.Id == id);
            }
            public Notification Save(Notification notification)
            {
                notification.Id = NextId();
                _notifications.Add(notification);
                _serializer.ToCSV(FilePath, _notifications);
                return notification;
            }

            public int NextId()
            {
                _notifications = _serializer.FromCSV(FilePath);
                if (_notifications.Count < 1)
                {
                    return 1;
                }
                return _notifications.Max(c => c.Id) + 1;
            }

            public void Delete(Notification notification)
            {
                Notification founded = _notifications.Find(a => a.Id == notification.Id);
                _notifications.Remove(founded);
                _serializer.ToCSV(FilePath, _notifications);
            }

            public Notification Update(Notification notification)
            {
                Notification current = _notifications.Find(a => a.Id == notification.Id);
                int index = _notifications.IndexOf(current);
                _notifications.Remove(current);
                _notifications.Insert(index, notification);       
                _serializer.ToCSV(FilePath, _notifications);
                return notification;
            }
    }
}