using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;

        public CommentService(ICommentRepository repository)
        {
            _repository = repository;
        }

        public List<CommentDTO> GetAllComments()
        {
            return _repository.GetAll()
                    .Select(comment => new CommentDTO(comment))
                    .ToList();
        }

        public CommentDTO AddComment(CommentDTO comment)
        {
            return new CommentDTO(_repository.Save(comment.ToComment()));
        }

        public void DeleteComment(CommentDTO comment)
        {
            _repository.Delete(comment.ToComment());
        }

        public CommentDTO UpdateComment(CommentDTO comment)
        {
            return new CommentDTO(_repository.Update(comment.ToComment()));
        }

        public List<CommentDTO> GetCommentsByUser(UserDTO user)
        {
            return _repository.GetByUser(user.ToUser())
                      .Select(comment => new CommentDTO(comment))
                      .ToList();
        }
    }
}
