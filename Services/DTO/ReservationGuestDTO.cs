using BookingApp.Domain.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class ReservationGuestDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _reservationId;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private int _age;
        private string _email = string.Empty;
        private bool _hasAppeared;
        private int _keyPointJoinedAt;
        private string _fullName = string.Empty;
        private string _appearanceStatus = string.Empty;
        private bool _isMainContact; 

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

        public int ReservationId
        {
            get => _reservationId;
            set { _reservationId = value; OnPropertyChanged(); }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value ?? string.Empty;
                UpdateFullName();
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value ?? string.Empty;
                UpdateFullName();
                OnPropertyChanged();
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set { _email = value ?? string.Empty; OnPropertyChanged(); }
        }

        public bool HasAppeared
        {
            get => _hasAppeared;
            set
            {
                _hasAppeared = value;
                UpdateAppearanceStatus();
                OnPropertyChanged();
            }
        }

        public int KeyPointJoinedAt
        {
            get => _keyPointJoinedAt;
            set { _keyPointJoinedAt = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get => _fullName;
            private set { _fullName = value; OnPropertyChanged(); }
        }

        public string AppearanceStatus
        {
            get => _appearanceStatus;
            private set { _appearanceStatus = value; OnPropertyChanged(); }
        }

        
        public bool IsMainContact
        {
            get => _isMainContact;
            set
            {
                _isMainContact = value;
                OnPropertyChanged();
            }
        }

        public ReservationGuest OriginalGuest { get; set; } = new ReservationGuest();

        public ReservationGuestDTO()
        {
            HasAppeared = false;
            KeyPointJoinedAt = -1;
            IsMainContact = false;
        }

        public ReservationGuestDTO(ReservationGuest guest)
        {
            if (guest == null)
                throw new ArgumentNullException(nameof(guest));

            Id = guest.Id;
            ReservationId = guest.ReservationId;
            FirstName = guest.FirstName;
            LastName = guest.LastName;
            Age = guest.Age;
            Email = guest.Email;
            HasAppeared = guest.HasAppeared;
            KeyPointJoinedAt = guest.KeyPointJoinedAt;

            
            IsMainContact = !string.IsNullOrEmpty(guest.Email);

            OriginalGuest = guest;
        }

        private void UpdateFullName()
        {
            FullName = $"{_firstName} {_lastName}".Trim();
        }

        private void UpdateAppearanceStatus()
        {
            AppearanceStatus = _hasAppeared ? "Pojavio se" : "Nije se pojavio";
        }

        public ReservationGuest ToReservationGuest()
        {
            return new ReservationGuest(Id, ReservationId, FirstName, LastName,
                                        Age, Email, KeyPointJoinedAt, HasAppeared);
        }

        public static ReservationGuestDTO FromDomain(ReservationGuest guest)
        {
            return new ReservationGuestDTO(guest);
        }

        public bool IsValidGuest()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   Age > 0 && Age < 120;
        }
    }
}