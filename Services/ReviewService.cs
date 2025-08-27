using System;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IAccommodationReviewRepository _ownerReviewRepository;
        private readonly IGuestReviewRepository _guestReviewRepository;
        private const int DaysToLeaveReview = 5; 

        public ReviewService(IAccommodationReviewRepository ownerReviewRepository, IGuestReviewRepository guestReviewRepository)
        {
            _ownerReviewRepository = ownerReviewRepository;
            _guestReviewRepository = guestReviewRepository;
        }
        public bool IsReviewPeriodExpired(Reservation reservation)
        {
            return (DateTime.Now - reservation.EndDate).TotalDays > DaysToLeaveReview; ;
        }
        public void CreateOwnerReview(Reservation reservation, int cleanliness, int ownerRating, string comment, string imagePaths)
        {
            if (IsReviewPeriodExpired(reservation))
            {
                throw new Exception($"You can only leave a review within {DaysToLeaveReview} days after your stay.");
            }

            var review = new AccommodationReview
            {
                ReservationId = reservation.Id,
                CleanlinessRating = cleanliness,
                OwnerRating = ownerRating,
                Comment = comment,
                ImagePaths = imagePaths,
                CreatedAt = DateTime.Now
            };

            _ownerReviewRepository.Save(review);
        }
        public bool HasGuestRated(int reservationId)
        {
            return _ownerReviewRepository.HasGuestRated(reservationId);
        }
    }
  }