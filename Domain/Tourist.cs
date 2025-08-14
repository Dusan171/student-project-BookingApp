using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Domain
{
    public class Tourist : User, ISerializable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Tourist() { }

        public Tourist(int id, string username, string password, UserRole role, string firstName, string lastName)
        {
            Id = id;
            Username = username;
            Password = password;
            Role = role;
            FirstName = firstName;
            LastName = lastName;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                Username,
                Password,
                Role.ToString(),
                FirstName,
                LastName
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            Username = values[1];
            Password = values[2];
            Role = Enum.Parse<UserRole>(values[3]);
            FirstName = values[4];
            LastName = values[5];
        }
    }
}

