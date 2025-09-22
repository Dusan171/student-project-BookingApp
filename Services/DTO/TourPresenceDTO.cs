using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;
using BookingApp.Repositories;

namespace BookingApp.Services.DTO
{
    public class TourPresenceDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _tourId;
        private string _tourName = string.Empty;
        private int _userId;
        private int _currentKeyPointIndex;
        private int _keyPointId;
        private string _currentKeyPointName = string.Empty;
        private int _totalKeyPoints;
        private List<string> _keyPointNames = new List<string>();
        private bool _isPresent;
        private DateTime _joinedAt;
        private DateTime _lastUpdated;
        private List<ReservationGuestDTO> _guests = new List<ReservationGuestDTO>();
        private TourStatus _tourStatus;
        private TourReservationStatus _reservationStatus;
        private string _progressText = string.Empty;
      
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
            set
            {
                _tourId = value;
                OnPropertyChanged();
            }
        }

        public string TourName
        {
            get => _tourName;
            set
            {
                _tourName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public int KeyPointId
        {
            get => _keyPointId;
            set
            {
                _keyPointId = value;
                UpdateCurrentKeyPointName();
                OnPropertyChanged();
            }
        }

        public int CurrentKeyPointIndex
        {
            get => _currentKeyPointIndex;
            set
            {
                _currentKeyPointIndex = value;
                UpdateProgressInfo();
                UpdateCurrentKeyPointName();
                OnPropertyChanged();
            }
        }

        public string CurrentKeyPointName
        {
            get => _currentKeyPointName;
            set
            {
                _currentKeyPointName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public int TotalKeyPoints
        {
            get => _totalKeyPoints;
            set
            {
                _totalKeyPoints = value;
                UpdateProgressInfo();
                OnPropertyChanged();
            }
        }

        public List<string> KeyPointNames
        {
            get => _keyPointNames;
            set
            {
                _keyPointNames = value ?? new List<string>();
                _totalKeyPoints = _keyPointNames.Count;
                UpdateProgressInfo();
                UpdateCurrentKeyPointName();
                OnPropertyChanged();
            }
        }

        public bool IsPresent
        {
            get => _isPresent;
            set
            {
                _isPresent = value;
                OnPropertyChanged();
            }
        }

        public DateTime JoinedAt
        {
            get => _joinedAt;
            set
            {
                _joinedAt = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set
            {
                _lastUpdated = value;
                OnPropertyChanged();
            }
        }

        public List<ReservationGuestDTO> Guests
        {
            get => _guests;
            set
            {
                _guests = value ?? new List<ReservationGuestDTO>();
                OnPropertyChanged();
            }
        }

        public TourStatus TourStatus
        {
            get => _tourStatus;
            set
            {
                _tourStatus = value;
                OnPropertyChanged();
            }
        }

        public TourReservationStatus ReservationStatus
        {
            get => _reservationStatus;
            set
            {
                _reservationStatus = value;
                OnPropertyChanged();
            }
        }

        public string ProgressText
        {
            get => _progressText;
            set { _progressText = value; OnPropertyChanged(); }
        }

        public string ProgressTextDetailed => $"{CurrentKeyPointIndex + 1} / {TotalKeyPoints}";

        public double ProgressPercentage => TotalKeyPoints > 0 ?
            (double)(CurrentKeyPointIndex + 1) / TotalKeyPoints * 100 : 0;

        public string StatusText
        {
            get
            {
                if (TourStatus == TourStatus.FINISHED)
                    return "Tura završena";
                if (!IsPresent)
                    return "Niste prisutni";
                if (CurrentKeyPointIndex < 0)
                    return "Početak ture";
                return $"Na tački: {CurrentKeyPointName}";
            }
        }

        public bool CanJoinTour => TourStatus == TourStatus.ACTIVE && !IsPresent;

        public bool IsActive => TourStatus == TourStatus.ACTIVE && IsPresent;

        public bool HasGuests => Guests?.Any() == true;

        public int TotalGuestsCount => Guests?.Count ?? 0;

        public int PresentGuestsCount => Guests?.Count(g => g.HasAppeared) ?? 0;

        public string NextKeyPointName
        {
            get
            {
                if (KeyPointNames == null || CurrentKeyPointIndex + 1 >= KeyPointNames.Count)
                    return "Kraj ture";
                return KeyPointNames[CurrentKeyPointIndex + 1];
            }
        }

        public bool HasNextKeyPoint => KeyPointNames != null && CurrentKeyPointIndex + 1 < KeyPointNames.Count;

        public List<string> RemainingKeyPoints
        {
            get
            {
                if (KeyPointNames == null || CurrentKeyPointIndex + 1 >= KeyPointNames.Count)
                    return new List<string>();

                return KeyPointNames.Skip(CurrentKeyPointIndex + 1).ToList();
            }
        }

        public Tour OriginalTour { get; set; }

        public TourPresenceDTO()
        {
            JoinedAt = DateTime.Now;
            LastUpdated = DateTime.Now;
        }

        public TourPresenceDTO(TourPresence presence)
        {
            if (presence == null)
                throw new ArgumentNullException(nameof(presence));

            Id = presence.Id;
            TourId = presence.TourId;
            UserId = presence.UserId;
            KeyPointId = presence.KeyPointId;
            JoinedAt = presence.JoinedAt;
            IsPresent = presence.IsPresent;
            CurrentKeyPointIndex = presence.CurrentKeyPointIndex;
            LastUpdated = presence.LastUpdated;
        }

        private void UpdateProgressInfo()
        {
            if (TotalKeyPoints > 0)
            {
                ProgressText = $"Ključna tačka {CurrentKeyPointIndex + 1} od {TotalKeyPoints}";
            }
            else if (OriginalTour?.KeyPoints != null && OriginalTour.KeyPoints.Count > 0)
            {
                ProgressText = $"Ključna tačka {CurrentKeyPointIndex + 1} od {OriginalTour.KeyPoints.Count}";
            }
            else
            {
                ProgressText = "Nema dostupnih ključnih tačaka";
            }

            OnPropertyChanged(nameof(ProgressPercentage));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(NextKeyPointName));
            OnPropertyChanged(nameof(HasNextKeyPoint));
            OnPropertyChanged(nameof(RemainingKeyPoints));
            OnPropertyChanged(nameof(ProgressTextDetailed));
        }

        private void UpdateCurrentKeyPointName()
        {
            if (KeyPointId > 0)
            {
                try
                {
                    KeyPointRepository keyPointRepository = new KeyPointRepository();
                    var keyPoint = keyPointRepository.GetById(KeyPointId);

                    if (keyPoint != null)
                    {
                        CurrentKeyPointName = keyPoint.Name;
                    }
                    else
                    {
                        CurrentKeyPointName = $"KeyPoint ID: {KeyPointId}";
                    }
                }
                catch (Exception ex)
                {
                    CurrentKeyPointName = $"Greška: {KeyPointId}";
                }
            }
            else if (CurrentKeyPointIndex < 0)
            {
                CurrentKeyPointName = "Početak";
            }
            else
            {
                CurrentKeyPointName = "Nepoznata tačka";
            }
        }

        public void RefreshKeyPointName()
        {
            UpdateCurrentKeyPointName();
        }

        public List<string> GetPassedKeyPoints()
        {
            if (KeyPointNames == null || CurrentKeyPointIndex < 0)
                return new List<string>();

            return KeyPointNames.Take(CurrentKeyPointIndex + 1).ToList();
        }

        public bool IsKeyPointPassed(int keyPointIndex)
        {
            return keyPointIndex <= CurrentKeyPointIndex;
        }

        public List<ReservationGuestDTO> GetGuestsJoinedAt(int keyPointIndex)
        {
            return Guests?.Where(g => g.KeyPointJoinedAt == keyPointIndex).ToList()
                   ?? new List<ReservationGuestDTO>();
        }
        

        public void SetKeyPointsFromTour(Tour tour)
        {
            if (tour?.KeyPoints != null)
            {
                KeyPointNames = tour.KeyPoints.Select(kp => kp.Name).ToList();
                OriginalTour = tour;
            }
        }

        public TourPresence ToTourPresence()
        {
            return new TourPresence(Id, TourId, UserId, JoinedAt, IsPresent, CurrentKeyPointIndex, KeyPointId);
        }

        public static TourPresenceDTO FromDomain(TourPresence presence)
        {
            return new TourPresenceDTO(presence);
        }

        public override string ToString()
        {
            return $"{TourName} - {StatusText} ({ProgressText})";
        }
    }
}