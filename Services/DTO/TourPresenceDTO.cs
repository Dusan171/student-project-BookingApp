using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class TourPresenceDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _tourId;
        private int _userId;
        private DateTime _joinedAt;
        private bool _isPresent;
        private int _currentKeyPointIndex;
        private DateTime _lastUpdated;
        private string _tourName = string.Empty;
        private string _currentKeyPointName = string.Empty;
        private string _progressText = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

        public int UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        public DateTime JoinedAt
        {
            get => _joinedAt;
            set { _joinedAt = value; OnPropertyChanged(); }
        }

        public bool IsPresent
        {
            get => _isPresent;
            set { _isPresent = value; OnPropertyChanged(); }
        }

        public int CurrentKeyPointIndex
        {
            get => _currentKeyPointIndex;
            set
            {
                _currentKeyPointIndex = value;
                OnPropertyChanged();
                UpdateProgressText();
            }
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set { _lastUpdated = value; OnPropertyChanged(); }
        }

        public string TourName
        {
            get => _tourName;
            set { _tourName = value; OnPropertyChanged(); }
        }

        public string CurrentKeyPointName
        {
            get => _currentKeyPointName;
            set { _currentKeyPointName = value; OnPropertyChanged(); }
        }

        public string ProgressText
        {
            get => _progressText;
            set { _progressText = value; OnPropertyChanged(); }
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
            JoinedAt = presence.JoinedAt;
            IsPresent = presence.IsPresent;
            CurrentKeyPointIndex = presence.CurrentKeyPointIndex;
            LastUpdated = presence.LastUpdated;
        }

        private void UpdateProgressText()
        {
            if (OriginalTour?.KeyPoints != null && OriginalTour.KeyPoints.Count > 0)
            {
                ProgressText = $"Ključna tačka {CurrentKeyPointIndex + 1} od {OriginalTour.KeyPoints.Count}";
            }
            else
            {
                ProgressText = "Nema dostupnih ključnih tačaka";
            }
        }

        public TourPresence ToTourPresence()
        {
            return new TourPresence(Id, TourId, UserId, JoinedAt, IsPresent, CurrentKeyPointIndex);
        }

        public static TourPresenceDTO FromDomain(TourPresence presence)
        {
            return new TourPresenceDTO(presence);
        }
    }

}
