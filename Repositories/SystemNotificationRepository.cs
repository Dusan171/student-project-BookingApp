using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class SystemNotificationRepository : ISystemNotificationRepository
    {
        private const string FilePath = "../../../Resources/Data/system_notifications.csv";
        private readonly Serializer<SystemNotification> _serializer;
        private List<SystemNotification> _notifications;

        public SystemNotificationRepository()
        {
            _serializer = new Serializer<SystemNotification>();
            _notifications = _serializer.FromCSV(FilePath);
        }

        public List<SystemNotification> GetAll() => _serializer.FromCSV(FilePath);

        public SystemNotification Save(SystemNotification notification)
        {
            _notifications = GetAll();
            notification.Id = _notifications.Any() ? _notifications.Max(n => n.Id) + 1 : 1;
            _notifications.Add(notification);
            _serializer.ToCSV(FilePath, _notifications);
            return notification;
        }
    }
}