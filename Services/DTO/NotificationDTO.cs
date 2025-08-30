using BookingApp.Domain.Model;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class NotificationDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _reservationId;
        private int _guestId;
        private DateTime _deadline;
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
        public int ReservationId
        {
            get => _reservationId;
            set
            {
                if (_reservationId != value)
                {
                    _reservationId = value;
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
        public DateTime Deadline
        {
            get => _deadline;
            set
            {
                if (_deadline != value)
                {
                    _deadline = value;
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
        public NotificationDTO() { }
        public NotificationDTO(Notification notification)
        {
            _id = notification.Id;
            _reservationId = notification.ReservationId;
            _guestId = notification.GuestId;
            _deadline = notification.Deadline;
            _isRead = notification.IsRead;
        }        
        public Notification ToNotification()
        {
            return new Notification
            {
                Id = _id,
                ReservationId = _reservationId,
                GuestId = _guestId,
                Deadline = _deadline,
                IsRead = _isRead
            };
        }
    }
}
