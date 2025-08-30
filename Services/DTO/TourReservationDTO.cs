using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class TourReservationDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _tourId;
        private int _startTourTimeId;
        private int _touristId;
        private int _numberOfGuests;
        private DateTime _reservationDate;
        private TourReservationStatus _status;
        private string _tourName = string.Empty;
        private string _guideName = string.Empty;
        private string _tourDateFormatted = string.Empty;
        private string _statusText = string.Empty;
        private List<ReservationGuestDTO> _guests = new List<ReservationGuestDTO>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public int TourId
        {
            get => _tourId;
            set { _tourId = value; OnPropertyChanged(); }
        }

        public int StartTourTimeId
        {
            get => _startTourTimeId;
            set { _startTourTimeId = value; OnPropertyChanged(); }
        }

        public int TouristId
        {
            get => _touristId;
            set { _touristId = value; OnPropertyChanged(); }
        }

        public int NumberOfGuests
        {
            get => _numberOfGuests;
            set { _numberOfGuests = value; OnPropertyChanged(); }
        }

        public DateTime ReservationDate
        {
            get => _reservationDate;
            set { _reservationDate = value; OnPropertyChanged(); }
        }

        public TourReservationStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                StatusText = GetStatusText(_status);
                OnPropertyChanged();
            }
        }

        public string TourName
        {
            get => _tourName;
            set { _tourName = value; OnPropertyChanged(); }
        }

        public string GuideName
        {
            get => _guideName;
            set { _guideName = value; OnPropertyChanged(); }
        }

        public string TourDateFormatted
        {
            get => _tourDateFormatted;
            set { _tourDateFormatted = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get => _statusText;
            private set { _statusText = value; OnPropertyChanged(); }
        }

        public List<ReservationGuestDTO> Guests
        {
            get => _guests;
            set { _guests = value; OnPropertyChanged(); }
        }

        public TourReservation OriginalReservation { get; set; } = new TourReservation();

        public TourReservationDTO()
        {
            Status = TourReservationStatus.ACTIVE;
            ReservationDate = DateTime.Now;
        }

        public TourReservationDTO(TourReservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));

            Id = reservation.Id;
            TourId = reservation.TourId;
            StartTourTimeId = reservation.StartTourTimeId;
            TouristId = reservation.TouristId;
            NumberOfGuests = reservation.NumberOfGuests;
            ReservationDate = reservation.ReservationDate;
            Status = reservation.Status;

            TourName = reservation.Tour?.Name ?? "Nepoznata tura";
            GuideName = GetGuideNameFromTour(reservation.Tour);
            TourDateFormatted = reservation.StartTourTime?.Time.ToString("dd.MM.yyyy") ??
                               reservation.ReservationDate.ToString("dd.MM.yyyy");

            Guests = reservation.Guests?.Select(g => new ReservationGuestDTO(g)).ToList() ??
                     new List<ReservationGuestDTO>();

            OriginalReservation = reservation;
        }

        private string GetGuideNameFromTour(Tour? tour)
        {
            if (tour?.Guide != null)
            {
                string firstName = tour.Guide.FirstName ?? "";
                string lastName = tour.Guide.LastName ?? "";
                return $"{firstName} {lastName}".Trim();
            }
            return "Nepoznat vodič";
        }

        private string GetStatusText(TourReservationStatus status)
        {
            return status switch
            {
                TourReservationStatus.ACTIVE => "Aktivna",
                TourReservationStatus.COMPLETED => "Završena",
                TourReservationStatus.CANCELLED => "Otkazana",
                _ => "Nepoznato"
            };
        }

        public TourReservation ToTourReservation()
        {
            return new TourReservation(Id, TourId, StartTourTimeId, TouristId,
                                     NumberOfGuests, ReservationDate, Status);
        }

        public static TourReservationDTO FromDomain(TourReservation reservation)
        {
            return new TourReservationDTO(reservation);
        }
    }
}