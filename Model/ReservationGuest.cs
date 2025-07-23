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
        public bool HasAppeared { get; set; }
        public int KeyPointJoinedAt { get; set; } // ID ključne tačke gde se gost pridružio turi

        public ReservationGuest()
        {
            HasAppeared = false;
            KeyPointJoinedAt = -1; // -1 znači da se još nije pridružio
        }

        public ReservationGuest(int id, int reservationId, string firstName, string lastName,
                               int age, bool hasAppeared = false, int keyPointJoinedAt = -1)
        {
            Id = id;
            ReservationId = reservationId;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            HasAppeared = hasAppeared;
            KeyPointJoinedAt = keyPointJoinedAt;
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
                HasAppeared.ToString(),
                KeyPointJoinedAt.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ReservationId = int.Parse(values[1]);
            FirstName = values[2];
            LastName = values[3];
            Age = int.Parse(values[4]);
            HasAppeared = bool.Parse(values[5]);
            KeyPointJoinedAt = int.Parse(values[6]);
        }
    }
}