using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class ReservationGuest : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; } // NOVO POLJE
        public bool HasAppeared { get; set; }
        public int KeyPointJoinedAt { get; set; }
        public int TouristId { get; set; }

        public ReservationGuest()
        {
            HasAppeared = false;
            KeyPointJoinedAt = -1;
        }

        public ReservationGuest(int id, int reservationId, string firstName, string lastName,
                               int age, string email, int touristId, bool hasAppeared = false, int keyPointJoinedAt = -1)
        {
            Id = id;
            ReservationId = reservationId;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Email = email;
            HasAppeared = hasAppeared;
            KeyPointJoinedAt = keyPointJoinedAt;
            TouristId = touristId;
        }

        public string FullName => $"{FirstName} {LastName}";

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ReservationId.ToString(),
                FirstName,
                LastName,
                Age.ToString(),
                Email ?? "",
                HasAppeared.ToString(),
                KeyPointJoinedAt.ToString(),
                TouristId.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ReservationId = int.Parse(values[1]);
            FirstName = values[2];
            LastName = values[3];
            Age = int.Parse(values[4]);

            if (values.Length > 7)
            {
                Email = values[5];
                HasAppeared = bool.Parse(values[6]);
                KeyPointJoinedAt = int.Parse(values[7]);
                TouristId = int.Parse(values[8]);
            }
            else
            {
                Email = "";
                HasAppeared = bool.Parse(values[5]);
                KeyPointJoinedAt = int.Parse(values[6]);
                TouristId = int.Parse(values[7]);
            }
        }
    }
}

