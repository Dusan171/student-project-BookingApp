using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
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

        public List<Comment> GetAllComments()
        {
            return _repository.GetAll();
        }

        public Comment AddComment(Comment comment)
        {
            return _repository.Save(comment);
        }

        public void DeleteComment(Comment comment)
        {
            _repository.Delete(comment);
        }

        public Comment UpdateComment(Comment comment)
        {
            return _repository.Update(comment);
        }

        public List<Comment> GetCommentsByUser(User user)
        {
            return _repository.GetByUser(user);
        }
    }
}
