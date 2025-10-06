using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class GuestRatingDetailsDTO : INotifyPropertyChanged
    {
        private GuestReviewDTO _review;
        private int _reservationId;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _guestName = string.Empty;
        private string _accommodationName = string.Empty;

        public GuestReviewDTO Review
        {
            get => _review;
            set
            {
                _review = value;
                OnPropertyChanged();
            }
        }

        public int ReservationId
        {
            get => _reservationId;
            set
            {
                _reservationId = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public string GuestName
        {
            get => _guestName;
            set
            {
                _guestName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string AccommodationName
        {
            get => _accommodationName;
            set
            {
                _accommodationName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public GuestRatingDetailsDTO()
        {
            Review = new GuestReviewDTO();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}