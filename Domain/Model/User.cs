using BookingApp.Serializer;
using System;

namespace BookingApp.Domain.Model
{
    public enum UserRole { OWNER, GUEST, GUIDE, TOURIST }

    public class User : ISerializable
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public User() { }

        public User(string username, string password, UserRole role, string firstName, string lastName)
        {
            Username = username ?? string.Empty;
            Password = password ?? string.Empty;
            Role = role;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
        }

        public string[] ToCSV()
        {
            string[] csvValues = {
                Id.ToString(),
                Username ?? string.Empty,
                Password ?? string.Empty,
                Role.ToString(),
                FirstName ?? string.Empty,
                LastName ?? string.Empty
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 4)
            {
                throw new ArgumentException("Invalid CSV data for User");
            }

            Id = Convert.ToInt32(values[0]);
            Username = values[1] ?? string.Empty;
            Password = values[2] ?? string.Empty;
            Role = (UserRole)Enum.Parse(typeof(UserRole), values[3]);

            FirstName = values.Length > 4 ? values[4] ?? string.Empty : string.Empty;
            LastName = values.Length > 5 ? values[5] ?? string.Empty : string.Empty;
        }
    }
}