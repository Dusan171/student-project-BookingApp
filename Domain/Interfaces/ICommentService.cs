using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface ICommentService
    {
        List<CommentDTO> GetAllComments();
        CommentDTO AddComment(CommentDTO comment);
        void DeleteComment(CommentDTO comment);
        CommentDTO UpdateComment(CommentDTO comment);
        List<CommentDTO> GetCommentsByUser(UserDTO user);
    }
}
