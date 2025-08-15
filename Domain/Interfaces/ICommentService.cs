using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface ICommentService
    {
        List<Comment> GetAllComments();
        Comment AddComment(Comment comment);
        void DeleteComment(Comment comment);
        Comment UpdateComment(Comment comment);
        List<Comment> GetCommentsByUser(User user);
    }
}
