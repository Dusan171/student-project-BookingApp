using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class ReservationDTO
    {
        // privatna polja
        private int _id;
        private int _accommodationId;
        private int _guestId;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _guestsNumber;
        private string _status;

        // javna svojstva
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int AccommodationId
        {
            get => _accommodationId;
            set => _accommodationId = value;
        }

        public int GuestId
        {
            get => _guestId;
            set => _guestId = value;
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value;
        }

        public int GuestsNumber
        {
            get => _guestsNumber;
            set => _guestsNumber = value;
        }

        public string Status
        {
            get => _status;
            set => _status = value;
        }

        // prazan konstruktor
        public ReservationDTO() { }

        // konstruktor koji prima entitet
        public ReservationDTO(Reservation reservation)
        {
            _id = reservation.Id;
            _accommodationId = reservation.AccommodationId;
            _guestId = reservation.GuestId;
            _startDate = reservation.StartDate;
            _endDate = reservation.EndDate;
            _guestsNumber = reservation.GuestsNumber;
            _status = reservation.Status.ToString();
        }

        // metoda koja DTO pretvara nazad u entitet
        public Reservation ToReservation()
        {
            return new Reservation
            {
                Id = _id,
                AccommodationId = _accommodationId,
                GuestId = _guestId,
                StartDate = _startDate,
                EndDate = _endDate,
                GuestsNumber = _guestsNumber,
                Status = Enum.TryParse<ReservationStatus>(_status, out var status) ? status : ReservationStatus.Active
            };
        }
    }
}