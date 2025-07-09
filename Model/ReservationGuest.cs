using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Model
{
    public class ReservationGuest : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public ReservationGuest() { }

        public ReservationGuest(string firstName, string lastName, int age)
        {
            Id = Id;
            ReservationId = ReservationId;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ReservationId.ToString(),
                FirstName,
                LastName,
                Age.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ReservationId = int.Parse(values[1]);
            FirstName = values[2];
            LastName = values[3];
            Age = int.Parse(values[4]);
        }
    }
}

