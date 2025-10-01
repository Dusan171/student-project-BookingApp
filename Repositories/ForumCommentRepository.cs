using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ForumCommentRepository : IForumCommentRepository
    {
        private const string FilePath = "../../../Resources/Data/forum_comments.csv";
        private readonly Serializer<ForumComment> _serializer;

        private List<ForumComment> _entities;

        public ForumCommentRepository()
        {
            _serializer = new Serializer<ForumComment>();
            _entities = _serializer.FromCSV(FilePath);
        }
        public List<ForumComment> GetAll()
        {
            return _entities;
        }
        public ForumComment GetById(int id)
        {
            return _entities.FirstOrDefault(fc => fc.Id == id);
        }
        public ForumComment Save(ForumComment forumComment)
        {
            forumComment.Id = NextId();
            _entities.Add(forumComment);
            SaveChanges(); 
            return forumComment;
        }
        public int NextId()
        {
            if (!_entities.Any())
            {
                return 1;
            }
            return _entities.Max(fc => fc.Id) + 1;
        }
        public void Delete(ForumComment forumComment)
        {
            var found = _entities.FirstOrDefault(fc => fc.Id == forumComment.Id);
            if (found != null)
            {
                _entities.Remove(found);
                SaveChanges();
            }
        }
        public ForumComment Update(ForumComment forumComment)
        {
            var current = _entities.FirstOrDefault(fc => fc.Id == forumComment.Id);
            if (current != null)
            {
                int index = _entities.IndexOf(current);
                _entities[index] = forumComment;
                SaveChanges();
            }
            return forumComment;
        }
        public List<ForumComment> GetByForumId(int forumId)
        {
            return _entities.Where(fc => fc.ForumId == forumId).ToList();
        }
        private void SaveChanges()
        {
            _serializer.ToCSV(FilePath, _entities);
        }
    }
}