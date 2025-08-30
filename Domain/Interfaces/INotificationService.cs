using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface INotificationService
    {
        List<NotificationDTO> GetAll();
        NotificationDTO GetById(int id);
        NotificationDTO Add(NotificationDTO notification);
        void Delete(NotificationDTO notification);
        NotificationDTO MarkAsRead(int id);
        void CheckAndGenerateNotifications();
    }
}
