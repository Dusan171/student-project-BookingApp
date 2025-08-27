using BookingApp.Serializer;
using System;

namespace BookingApp.Domain.Model
{
    public enum UserRole { OWNER, GUEST, GUIDE, TOURIST }

    public class User : ISerializable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User() { }

        public User(string username, string password, UserRole role,
                    string firstName, string lastName)
        {
            Username = username;
            Password = password;
            Role = role;
            FirstName = firstName;
            LastName = lastName;
        }

        public string[] ToCSV()
        {
            string[] csvValues = {
                Id.ToString(),
                Username,
                Password,
                Role.ToString(),
                FirstName,
                LastName
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Username = values[1];
            Password = values[2];
            Role = (UserRole)Enum.Parse(typeof(UserRole), values[3]);

            
            FirstName = values.Length > 4 ? values[4] : "";
            LastName = values.Length > 5 ? values[5] : "";
        }
    }
}
