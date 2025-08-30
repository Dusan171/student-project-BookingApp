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
                               int age, string email, bool hasAppeared = false, int keyPointJoinedAt = -1)
        {
            Id = id;
            ReservationId = reservationId;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Age = age;
            Email = email ?? string.Empty;
            HasAppeared = hasAppeared;
            KeyPointJoinedAt = keyPointJoinedAt;
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
                KeyPointJoinedAt.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 6)
            {
                throw new ArgumentException("Invalid CSV data for ReservationGuest");
            }

            Id = int.Parse(values[0]);
            ReservationId = int.Parse(values[1]);
            FirstName = values[2] ?? string.Empty;
            LastName = values[3] ?? string.Empty;
            Age = int.Parse(values[4]);

            if (values.Length >= 8)
            {
                Email = values[5] ?? string.Empty;
                HasAppeared = bool.Parse(values[6]);
                KeyPointJoinedAt = int.Parse(values[7]);
            }
            else if (values.Length >= 7)
            {
                Email = string.Empty;
                HasAppeared = bool.Parse(values[5]);
                KeyPointJoinedAt = int.Parse(values[6]);
            }
            else
            {
                Email = string.Empty;
                HasAppeared = false;
                KeyPointJoinedAt = -1;
            }
        }
    }
}