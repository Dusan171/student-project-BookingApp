using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IForumCommentRepository
    {
        ForumComment GetById(int id);
        List<ForumComment> GetAll();
        ForumComment Save(ForumComment forumComment);
        void Delete(ForumComment forumComment);
        ForumComment Update(ForumComment forumComment);
        List<ForumComment> GetByForumId(int forumId); // Ovo je naša specifična metoda
    }
}