using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class GuestAttendanceStatusDTO : INotifyPropertyChanged
    {
        private int _guestId;
        private string _guestName = string.Empty;
        private bool _isMainContact;
        private string _mainTouristName = string.Empty;
        private bool _hasAppeared;
        private int _keyPointJoinedAt;
        private string _attendanceStatus = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int GuestId
        {
            get => _guestId;
            set
            {
                _guestId = value;
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

        public bool IsMainContact
        {
            get => _isMainContact;
            set
            {
                _isMainContact = value;
                UpdateAttendanceStatus();
                OnPropertyChanged();
            }
        }

        public string MainTouristName
        {
            get => _mainTouristName;
            set
            {
                _mainTouristName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public bool HasAppeared
        {
            get => _hasAppeared;
            set
            {
                _hasAppeared = value;
                UpdateAttendanceStatus();
                OnPropertyChanged();
            }
        }

        public int KeyPointJoinedAt
        {
            get => _keyPointJoinedAt;
            set
            {
                _keyPointJoinedAt = value;
                OnPropertyChanged();
            }
        }

        public string AttendanceStatus
        {
            get => _attendanceStatus;
            private set
            {
                _attendanceStatus = value;
                OnPropertyChanged();
            }
        }

        public bool ShouldNotifyMainTourist => HasAppeared && !IsMainContact;

        public string DisplayNameForGuide
        {
            get
            {
                if (IsMainContact)
                {
                    return $"{GuestName} (glavni kontakt)";
                }
                else
                {
                    return $"{GuestName} (pripada: {MainTouristName})";
                }
            }
        }

        public GuestAttendanceStatusDTO()
        {
            UpdateAttendanceStatus();
        }

        private void UpdateAttendanceStatus()
        {
            if (HasAppeared)
            {
                AttendanceStatus = IsMainContact ?
                    "Prisutan (glavni kontakt)" :
                    "Prisutan";
            }
            else
            {
                AttendanceStatus = "Nije se pojavio";
            }
        }

        public bool BelongsToTourist(int touristId, string touristName)
        {
            return MainTouristName == touristName;
        }
    }
}