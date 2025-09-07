using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class TourPresenceNotificationDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _tourId;
        private int _userId;
        private string _message;
        private DateTime _createdAt;
        private bool _isRead;

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

        public int TourId
        {
            get => _tourId;
            set
            {
                if (_tourId != value)
                {
                    _tourId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                if (_userId != value)
                {
                    _userId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRead
        {
            get => _isRead;
            set
            {
                if (_isRead != value)
                {
                    _isRead = value;
                    OnPropertyChanged();
                }
            }
        }

        public TourPresenceNotificationDTO() { }

        public TourPresenceNotificationDTO(TourPresenceNotification notification)
        {
            _id = notification.Id;
            _tourId = notification.TourId;
            _userId = notification.UserId;
            _message = notification.Message;
            _createdAt = notification.CreatedAt;
            _isRead = notification.IsRead;
        }

        public TourPresenceNotification ToTourPresenceNotification()
        {
            return new TourPresenceNotification
            {
                Id = _id,
                TourId = _tourId,
                UserId = _userId,
                Message = _message,
                CreatedAt = _createdAt,
                IsRead = _isRead
            };
        }
    }
}