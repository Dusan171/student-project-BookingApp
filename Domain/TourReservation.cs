using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;

namespace BookingApp.Domain
{
    public class TourReservation : ISerializable
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public DateTime ReservationTime { get; set; }
        public bool IsActive { get; set; }


        public Tour Tour { get; set; }
        public Tourist Tourist { get; set; }
        public List<ReservationGuest> Guests { get; set; }

        public TourReservation()
        {
            Guests = new List<ReservationGuest>();
        }

        public TourReservation(int id, int tourId, int touristId, DateTime reservationTime, bool isActive)
        {
            Id = id;
            TourId = tourId;
            TouristId = touristId;
            ReservationTime = reservationTime;
            IsActive = isActive;
            Guests = new List<ReservationGuest>();
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                Id.ToString(),
                TourId.ToString(),
                TouristId.ToString(),
                ReservationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                IsActive.ToString()
            };
        }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            TourId = int.Parse(values[1]);
            TouristId = int.Parse(values[2]);
            ReservationTime = DateTime.Parse(values[3]);
            IsActive = bool.Parse(values[4]);
            Guests = new List<ReservationGuest>(); // Odrvojeno učitavanje iz posebnog CSV-a ako treba
        }
    }
}
