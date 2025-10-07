using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IForumNotificationService
    {
        void NotifyOwnersAboutNewForum(int forumId, string forumTitle, int locationId, string locationName);
        List<ForumNotificationDTO> GetUnreadForOwner(int ownerId);
        void MarkAsRead(int notificationId);
    }
}