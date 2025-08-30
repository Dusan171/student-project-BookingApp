using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.DTO;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class AccommodationReviewService : IAccommodationReviewService
    {
        
        private readonly IAccommodationReviewRepository _accommodationReviewRepository;
        private const int DaysToLeaveReview = 5;

        public AccommodationReviewService(IAccommodationReviewRepository accommodationReviewRepository)
        {
            _accommodationReviewRepository = accommodationReviewRepository;
        }

        public bool IsReviewPeriodExpired(ReservationDTO reservation)
        {
            return (DateTime.Now - reservation.EndDate).TotalDays > DaysToLeaveReview;
        }

        public void Create(ReservationDTO reservation, int cleanliness, int ownerRating, string comment, string imagePaths)
        {
            if (IsReviewPeriodExpired(reservation))
            {
                throw new Exception($"You can only leave a review within {DaysToLeaveReview} days after your stay.");
            }

            var review = new AccommodationReviewDTO
            {
                ReservationId = reservation.Id,
                CleanlinessRating = cleanliness,
                OwnerRating = ownerRating,
                Comment = comment,
                ImagePaths = imagePaths,
                CreatedAt = DateTime.Now
            };

            _accommodationReviewRepository.Save(review.ToReview());
        }

        public bool HasGuestRated(int reservationId)
        {
            return _accommodationReviewRepository.HasGuestRated(reservationId);
        }
        public AccommodationReview GetByReservationId(int reservationId)
        {
            var reviewsForReservation = _accommodationReviewRepository.GetByReservationId(reservationId);

            return reviewsForReservation.FirstOrDefault();
        }

        public List<AccommodationReviewDTO> GetAll()
        {
            var reviews = _accommodationReviewRepository.GetAll();
            return reviews.Select(r => new AccommodationReviewDTO(r)).ToList();
        }
        public void SubmitReview(CreateAccommodationReviewDTO reviewDto, DateTime reservationEndDate)
        {
            if ((DateTime.Now - reservationEndDate).TotalDays > DaysToLeaveReview)
            {
                throw new InvalidOperationException("Review period has expired.");
            }

            var reviewToSave = new AccommodationReview
            {
                ReservationId = reviewDto.ReservationId,
                CleanlinessRating = reviewDto.CleanlinessRating,
                OwnerRating = reviewDto.OwnerRating,
                Comment = reviewDto.Comment,
                ImagePaths = reviewDto.ImagePaths,
                CreatedAt = DateTime.Now
            };
            _accommodationReviewRepository.Save(reviewToSave);
        }
        public AccommodationReviewDetailsDTO GetDetailsByReservationId(int reservationId)
        {
            var reviewModel = _accommodationReviewRepository.GetByReservationId(reservationId).FirstOrDefault();

            if (reviewModel == null)
            {
                return null;
            }

            return new AccommodationReviewDetailsDTO
            {
                Id = reviewModel.Id,
                ReservationId = reviewModel.ReservationId,
                CleanlinessRating = reviewModel.CleanlinessRating,
                OwnerRating = reviewModel.OwnerRating,
                Comment = reviewModel.Comment,
                CreatedAt = reviewModel.CreatedAt
            };
        }
    }
}