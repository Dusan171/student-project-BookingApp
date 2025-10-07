using System;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class TourNotification : ISerializable
    {
        public int Id { get; set; }
        public int TouristId { get; set; }
        public int TourId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public TourNotification()
        {
            CreatedAt = DateTime.Now;
            IsRead = false;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                TouristId.ToString(),
                TourId.ToString(),
                Title,
                Message,
                CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"),
                IsRead.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 7)
                throw new ArgumentException("Invalid CSV data for TourNotification");

            Id = int.Parse(values[0]);
            TouristId = int.Parse(values[1]);
            TourId = int.Parse(values[2]);
            Title = values[3];
            Message = values[4];

            
            CreatedAt = DateTime.Parse(values[5]);

            IsRead = bool.Parse(values[6]);
        }
    }
}