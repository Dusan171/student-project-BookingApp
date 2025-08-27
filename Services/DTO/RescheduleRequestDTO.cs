using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class RescheduleRequestDTO : INotifyPropertyChanged
    {
        
        private int _id;
        private int _reservationId;
        private int _guestId;
        private DateTime _newStartDate;
        private DateTime _newEndDate;
        private RequestStatus _status;
        private string _ownerComment;
        private bool _isSeenByGuest;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
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

        public DateTime NewStartDate
        {
            get => _newStartDate;
            set
            {
                if (_newStartDate != value)
                {
                    _newStartDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime NewEndDate
        {
            get => _newEndDate;
            set
            {
                if (_newEndDate != value)
                {
                    _newEndDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public RequestStatus Status
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

        public string OwnerComment
        {
            get => _ownerComment;
            set
            {
                if (_ownerComment != value)
                {
                    _ownerComment = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSeenByGuest
        {
            get => _isSeenByGuest;
            set
            {
                if (_isSeenByGuest != value)
                {
                    _isSeenByGuest = value;
                    OnPropertyChanged();
                }
            }
        }
        public string AccommodationName { get; set; }
        public DateTime OriginalStartDate { get; set; }
        public DateTime OriginalEndDate { get; set; }
        public string AvailabilityStatus { get; set; }
        public RescheduleRequestDTO()
        {
            OwnerComment = string.Empty;
        }

        public RescheduleRequestDTO(RescheduleRequest request)
        {
            Id = request.Id;
            ReservationId = request.ReservationId;
            GuestId = request.GuestId;
            NewStartDate = request.NewStartDate;
            NewEndDate = request.NewEndDate;
            Status = request.Status;
            OwnerComment = request.OwnerComment;
            IsSeenByGuest = request.IsSeenByGuest;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RescheduleRequest ToRequest()
        {
            return new RescheduleRequest
            {
                Id = this.Id,
                ReservationId = this.ReservationId,
                GuestId = this.GuestId,
                NewStartDate = this.NewStartDate,
                NewEndDate = this.NewEndDate,
                Status = this.Status,
                OwnerComment = this.OwnerComment,
                IsSeenByGuest = this.IsSeenByGuest
            };
        }
    }
}
