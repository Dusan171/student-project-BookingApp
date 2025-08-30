using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
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

        public User? GetByUsername(string username)
        {
            _users = _serializer.FromCSV(FilePath) ?? new List<User>();
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public User? GetById(int id)
        {
            _users = _serializer.FromCSV(FilePath) ?? new List<User>();
            return _users.FirstOrDefault(u => u.Id == id);
        }
    }
}