using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class TourRequestParticipant : ISerializable
    {
        public int Id { get; set; }
        public int TourRequestId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }

        public TourRequestParticipant() { }

        public TourRequestParticipant(int id, int tourRequestId, string firstName, string lastName, int age)
        {
            Id = id;
            TourRequestId = tourRequestId;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Age = age;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                TourRequestId.ToString(),
                FirstName,
                LastName,
                Age.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 5)
                throw new ArgumentException("Invalid CSV data for TourRequestParticipant");

            Id = int.Parse(values[0]);
            TourRequestId = int.Parse(values[1]);
            FirstName = values[2] ?? string.Empty;
            LastName = values[3] ?? string.Empty;
            Age = int.Parse(values[4]);
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, {Age} god.";
        }
    }
}
