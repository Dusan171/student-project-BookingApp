using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public enum TourReservationStatus
    {
        ACTIVE,
        COMPLETED,
        CANCELLED
    }

    public class TourReservation : ISerializable
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int StartTourTimeId { get; set; }
        public int TouristId { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDate { get; set; }
        public TourReservationStatus Status { get; set; }

        public int CurrentKeyPointIndex { get; set; } = 0;
        public List<ReservationGuest> Guests { get; set; }
        public Tour? Tour { get; set; }
        public StartTourTime? StartTourTime { get; set; }


        public string TourName => Tour?.Name ?? "Nepoznata tura";

        public string GuideName
        {
            get
            {
                if (Tour?.Guide != null)
                {
                    string firstName = Tour.Guide.FirstName ?? "";
                    string lastName = Tour.Guide.LastName ?? "";
                    return $"{firstName} {lastName}".Trim();
                }
                return "Nepoznat vodič";
            }
        }

        public string TourDateFormatted => StartTourTime?.Time.ToString("dd.MM.yyyy") ?? ReservationDate.ToString("dd.MM.yyyy");

        public bool IsCompleted => Status == TourReservationStatus.COMPLETED;
        public TourReservation()
        {
            Guests = new List<ReservationGuest>();
            Status = TourReservationStatus.ACTIVE;
            ReservationDate = DateTime.Now;
        }

        public TourReservation(int id, int tourId, int startTourTimeId, int touristId,
                              int numberOfGuests, DateTime reservationDate, TourReservationStatus status)
        {
            Id = id;
            TourId = tourId;
            StartTourTimeId = startTourTimeId;
            TouristId = touristId;
            NumberOfGuests = numberOfGuests;
            ReservationDate = reservationDate;
            Status = status;
            Guests = new List<ReservationGuest>();
        }

        public string[] ToCSV()
        {
            return new string[]
            {
        Id.ToString(),
        TourId.ToString(),
        StartTourTimeId.ToString(),
        TouristId.ToString(),
        NumberOfGuests.ToString(),
        ReservationDate.ToString("dd-MM-yyyy HH:mm:ss"),
        Status.ToString(),
        string.Join(",", Guests.Select(g => g.Id)), // gosti
        CurrentKeyPointIndex.ToString()              // indeks ključne tačke
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 7)
                throw new ArgumentException("Invalid CSV data for TourReservation");

            Id = int.Parse(values[0]);
            TourId = int.Parse(values[1]);
            StartTourTimeId = int.Parse(values[2]);
            TouristId = int.Parse(values[3]);
            NumberOfGuests = int.Parse(values[4]);
            ReservationDate = DateTime.ParseExact(values[5], "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            Status = (TourReservationStatus)Enum.Parse(typeof(TourReservationStatus), values[6]);

            // Gosti
            Guests = new List<ReservationGuest>();
            if (values.Length > 7 && !string.IsNullOrEmpty(values[7]))
            {
                Guests = values[7].Split(',')
                                  .Select(s => new ReservationGuest { Id = int.Parse(s) })
                                  .ToList();
            }

            // Trenutni ključni indeks
            CurrentKeyPointIndex = values.Length > 8 ? int.Parse(values[8]) : 0;
        }


    }
}