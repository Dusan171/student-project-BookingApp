using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IOwnerForumService
    {
        bool CanOwnerComment(int forumId, int ownerId);
        Comment AddOwnerComment(int forumId, int ownerId, string commentText);
        List<int> GetForumsWhereOwnerCanComment(int ownerId);
    }
}