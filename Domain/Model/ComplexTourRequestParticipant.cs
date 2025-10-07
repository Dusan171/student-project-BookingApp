using System;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class ComplexTourRequestParticipant : ISerializable
    {
        public int Id { get; set; }
        public int ComplexTourRequestPartId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }

        public ComplexTourRequestParticipant() { }

        public ComplexTourRequestParticipant(int id, int complexTourRequestPartId, string firstName, string lastName, int age)
        {
            Id = id;
            ComplexTourRequestPartId = complexTourRequestPartId;
            FirstName = firstName ?? string.Empty;
            LastName = lastName ?? string.Empty;
            Age = age;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                ComplexTourRequestPartId.ToString(),
                FirstName,
                LastName,
                Age.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 5)
                throw new ArgumentException("Invalid CSV data for ComplexTourRequestParticipant");

            Id = int.Parse(values[0]);
            ComplexTourRequestPartId = int.Parse(values[1]);
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