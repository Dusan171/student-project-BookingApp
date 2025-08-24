using System;
using BookingApp.Domain;

namespace BookingApp.DTO
{
    public class AccommodationReviewDTO
    {
        private int _id;
        private int _reservationId;
        private int _cleanlinessRating;
        private int _ownerRating;
        private string _comment;
        private string _imagePaths;
        private DateTime _createdAt;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public int ReservationId
        {
            get => _reservationId;
            set => _reservationId = value;
        }

        public int CleanlinessRating
        {
            get => _cleanlinessRating;
            set => _cleanlinessRating = value;
        }

        public int OwnerRating
        {
            get => _ownerRating;
            set => _ownerRating = value;
        }

        public string Comment
        {
            get => _comment;
            set => _comment = value;
        }

        public string ImagePaths
        {
            get => _imagePaths;
            set => _imagePaths = value;
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = value;
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