using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface INotificationRepository
    {
        List<Notification> GetAll();
        Notification GetById(int id);
        Notification Save(Notification notification);
        void Delete(Notification notification);
        int NextId();
    }
}
