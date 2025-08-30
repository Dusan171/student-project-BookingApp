using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string FilePath = "../../../Resources/Data/users.csv";
        private readonly Serializer<User> _serializer;
        private List<User> _users;

        public UserRepository()
        {
            _serializer = new Serializer<User>();
            _users = _serializer.FromCSV(FilePath) ?? new List<User>();
        }

        private void SaveAll() => _serializer.ToCSV(FilePath, _users);

        private void Reload() => _users = _serializer.FromCSV(FilePath) ?? new List<User>();

        public List<User> GetAll() => _users.ToList();

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public User? GetByUsername(string username)
            => _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        public User Add(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.Id = GetNextId();
            _users.Add(user);
            SaveAll();
            return user;
        }

        public User Update(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            int index = _users.FindIndex(u => u.Id == user.Id);
            if (index >= 0)
            {
                _users[index] = user;
                SaveAll();
            }
            return user;
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _users.Remove(user);
                SaveAll();
            }
        }

        public int GetNextId() => _users.Count == 0 ? 1 : _users.Max(u => u.Id) + 1;
    }
}
