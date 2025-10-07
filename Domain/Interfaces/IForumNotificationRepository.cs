using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IForumNotificationRepository
    {
        ForumNotification Save(ForumNotification notification);
        List<ForumNotification> GetAll();
        List<ForumNotification> GetByOwnerId(int ownerId);
        List<ForumNotification> GetUnreadByOwnerId(int ownerId);
        ForumNotification GetById(int id);
        ForumNotification Update(ForumNotification notification);
    }
}