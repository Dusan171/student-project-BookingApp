using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ISystemNotificationRepository
    {
        List<SystemNotification> GetAll();
        SystemNotification Save(SystemNotification notification);
    }
}