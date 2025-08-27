using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class ReservationDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _accommodationId;
        private int _guestId;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _guestsNumber;
        private string _status;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int AccommodationId
        {
            get => _accommodationId;
            set
            {
                if (_accommodationId != value)
                {
                    _accommodationId = value;
                    OnPropertyChanged();
        }
            }
        }

        public int GuestId
        {
            get => _guestId;
            set
            {
                if (_guestId != value)
                {
                    _guestId = value;
                    OnPropertyChanged();
        }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public int GuestsNumber
        {
            get => _guestsNumber;
            set
            {
                if (_guestsNumber != value)
                {
                    _guestsNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public ReservationDTO() { }

        
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