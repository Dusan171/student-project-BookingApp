using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ForumRepository : IForumRepository
    {
        private const string FilePath = "../../../Resources/Data/forums.csv";
        private readonly Serializer<Forum> _serializer;

        private List<Forum> _entities;

        public ForumRepository()
        {
            _serializer = new Serializer<Forum>();
            _entities = _serializer.FromCSV(FilePath);
        }
        public List<Forum> GetAll()
        {
            return _entities;
        }
        public Forum GetById(int id)
        {
            return _entities.FirstOrDefault(f => f.Id == id);
        }
        public Forum Save(Forum forum)
        {
            forum.Id = NextId();
            _entities.Add(forum);
            SaveChanges(); 
            return forum;
        }
        public int NextId()
        {
            if (!_entities.Any())
            {
                return 1;
            }
            return _entities.Max(f => f.Id) + 1;
        }
        public void Delete(Forum forum)
        {
            var found = _entities.FirstOrDefault(f => f.Id == forum.Id);
            if (found != null)
            {
                _entities.Remove(found);
                SaveChanges();
            }
        }
        public Forum Update(Forum forum)
        {
            var current = _entities.FirstOrDefault(f => f.Id == forum.Id);
            if (current != null)
            {
                int index = _entities.IndexOf(current);
                _entities[index] = forum;
                SaveChanges();
            }
            return forum;
        }
        private void SaveChanges()
        {
            _serializer.ToCSV(FilePath, _entities);
        }
    }
}