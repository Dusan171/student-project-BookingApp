using BookingApp.Domain.Model;

namespace BookingApp.Domain.Interfaces
{
    public interface IForumManagementService
    {
        Forum Create(string title, string locationName, string firstCommentText);
        Comment AddComment(int forumId, string commentText);
        void CloseForum(int forumId);
    }
}