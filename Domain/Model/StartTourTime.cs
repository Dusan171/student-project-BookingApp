using System;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class StartTourTime : ISerializable
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }

        public int TourId { get; set; }

        public StartTourTime()
        {
        }

        public StartTourTime(int id, DateTime time, int tourId)
        {
            Id = id;
            Time = time;
            TourId = tourId;
        }
        public string[] ToCSV()
        {
            return new string[]
            {
        Id.ToString(),
        Time.ToString("yyyy-MM-dd HH:mm:ss"),
        TourId.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 3)
                throw new ArgumentException("Invalid CSV data for StartTourTime");

            Id = int.Parse(values[0]);
            Time = DateTime.ParseExact(values[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            TourId = int.Parse(values[2]);
        }

        public override string ToString()
        {
            return Time.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}