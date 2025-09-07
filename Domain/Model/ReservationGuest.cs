using System;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class ReservationGuest : ISerializable
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool HasAppeared { get; set; }
        public int KeyPointJoinedAt { get; set; }
        public int TouristId { get; set; }

        public string DisplayTitle
        {
            get
            {
                // Ovo će se postaviti u ViewModel-u kada se kreira guest
                return $"Osoba {Index + 1}" + (Email != null ? " (glavni kontakt)" : "");
            }
        }

        public int Index { get; set; }

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
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Age = age;
            Email = email ?? string.Empty;
            HasAppeared = hasAppeared;
            KeyPointJoinedAt = keyPointJoinedAt;
            TouristId = touristId;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ReservationId.ToString(),
                FirstName ?? string.Empty,
                LastName ?? string.Empty,
                Age.ToString(),
                Email ?? string.Empty,
                HasAppeared.ToString(),
                KeyPointJoinedAt.ToString(),
                TouristId.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 6)
            throw new ArgumentException("Invalid CSV data for ReservationGuest");

            Id = int.Parse(values[0]);
            ReservationId = int.Parse(values[1]);
            FirstName = values[2] ?? string.Empty;
            LastName = values[3] ?? string.Empty;
            Age = int.Parse(values[4]);
            Email = values.Length > 5 ? values[5] ?? string.Empty : string.Empty;
            HasAppeared = values.Length > 6 ? bool.Parse(values[6]) : false;
            KeyPointJoinedAt = values.Length > 7 ? int.Parse(values[7]) : -1;
            TouristId = values.Length > 8 ? int.Parse(values[8]) : 0;
        }

    }
}