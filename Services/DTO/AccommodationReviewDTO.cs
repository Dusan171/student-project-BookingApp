using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;

namespace BookingApp.DTO
{
    public class AccommodationReviewDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _reservationId;
        private int _cleanlinessRating;
        private int _ownerRating;
        private string _comment;
        private string _imagePaths;
        private DateTime _createdAt;
        private int _imageCount;

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

        public int CleanlinessRating
        {
            get => _cleanlinessRating;
            set
            {
                if (_cleanlinessRating != value)
                {
                    _cleanlinessRating = value;
                    OnPropertyChanged();
        }
            }
        }

        public int OwnerRating
        {
            get => _ownerRating;
            set
            {
                if (_ownerRating != value)
                {
                    _ownerRating = value;
                    OnPropertyChanged();
        }
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ImagePaths
        {
            get => _imagePaths;
            set
            {
                if (_imagePaths != value)
                {
                    _imagePaths = value;

                    _imageCount = string.IsNullOrWhiteSpace(value) ? 0 : value.Split(';', StringSplitOptions.RemoveEmptyEntries).Length;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ImageCount));
                }
            }
        }

        public int ImageCount
        {
            get => _imageCount;
            private set 
            {
                if (_imageCount != value)
                {
                    _imageCount = value;
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

        public AccommodationReviewDTO() { }

        public AccommodationReviewDTO(
            int id,
            int reservationId,
            int cleanlinessRating,
            int ownerRating,
            string comment,
            string imagePaths,
            DateTime createdAt)
        {
            _id = id;
            _reservationId = reservationId;
            _cleanlinessRating = cleanlinessRating;
            _ownerRating = ownerRating;
            _comment = comment;
            _imagePaths = imagePaths;
            _createdAt = createdAt;
        }

        public AccommodationReviewDTO(AccommodationReview review)
        {
            Id = review.Id;
            ReservationId = review.ReservationId;
            CleanlinessRating = review.CleanlinessRating;
            OwnerRating = review.OwnerRating;
            Comment = review.Comment;
            ImagePaths = review.ImagePaths;
            CreatedAt = review.CreatedAt;
        }

        public AccommodationReview ToReview()
        {
            return new AccommodationReview
            {
                Id = _id,
                ReservationId = _reservationId,
                CleanlinessRating = _cleanlinessRating,
                OwnerRating = _ownerRating,
                Comment = _comment,
                ImagePaths = _imagePaths,
                CreatedAt = _createdAt
            };
        }
    }
}