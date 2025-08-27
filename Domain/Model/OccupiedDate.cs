using System;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class OccupiedDate : ISerializable
    {
        public int Id { get; set; }
        public int AccommodationId { get; set; }
        public int ReservationId { get; set; }
        public DateTime Date { get; set; }

        public OccupiedDate() { }

        public OccupiedDate(int id, int accommodationId, int reservationId, DateTime date)
        {
            Id = id;
            AccommodationId = accommodationId;
            ReservationId = reservationId;
            Date = date;
        }
        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                AccommodationId.ToString(),
                ReservationId.ToString(),
                Date.ToString("yyyy-MM-dd")
            };
        }
        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            AccommodationId = int.Parse(values[1]);
            ReservationId = int.Parse(values[2]);
            Date = DateTime.Parse(values[3]);
        }
    }
}
