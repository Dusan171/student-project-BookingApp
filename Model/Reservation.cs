using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Model
{
    public class Reservation : ISerializable
    {
        public int Id { get; set; }
        public int AccommodationId { get; set; }
        public int GuestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfGuests { get; set; }
        public ReservationStatus Status { get; set; }

        public Reservation() { }

        public Reservation(int accommodationId, int guestId, DateTime startDate, DateTime endDate, int numberOfGuests)
        {
            AccommodationId = accommodationId;
            GuestId = guestId;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfGuests = numberOfGuests;
            Status = ReservationStatus.Active;

        }
        public string[] ToCSV()
        {
            return new string[] 
            {
                Id.ToString(),
                AccommodationId.ToString(),
                GuestId.ToString(),
                StartDate.ToString("o"),
                EndDate.ToString("o"),
                NumberOfGuests.ToString(),
                Status.ToString()
            };
        }
        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            AccommodationId = int.Parse(values[1]);
            GuestId = int.Parse(values[2]);
            StartDate = DateTime.Parse(values[3]);
            EndDate = DateTime.Parse(values[4]);
            NumberOfGuests = int.Parse(values[5]);
            Status = Enum.Parse<ReservationStatus>(values[6]);
        }
    }
}
