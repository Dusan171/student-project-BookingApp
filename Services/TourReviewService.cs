using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class TourReviewService : ITourReviewService
    {
        private readonly ITourReviewRepository _reviewRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IUserRepository _userRepository;

        public TourReviewService(ITourReviewRepository reviewRepository, ITourRepository tourRepository, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
            _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<TourReviewDTO> GetAllReviews()
        {
            var reviews = _reviewRepository.GetAll();
            return EnrichReviews(reviews);
        }

        public TourReviewDTO? GetReviewById(int id)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null) return null;
            EnrichReview(review);
            return new TourReviewDTO(review);
        }

        public List<TourReviewDTO> GetReviewsByTour(int tourId)
        {
            var reviews = _reviewRepository.GetByTourId(tourId);
            return EnrichReviews(reviews);
        }

        public List<TourReviewDTO> GetReviewsByTourist(int touristId)
        {
            var reviews = _reviewRepository.GetByTouristId(touristId);
            return EnrichReviews(reviews);
        }

        public TourReviewDTO AddReview(TourReviewDTO reviewDTO)
        {
            if (reviewDTO == null)
                throw new ArgumentNullException(nameof(reviewDTO));

            if (_reviewRepository.HasReview(reviewDTO.TouristId, reviewDTO.TourId))
                throw new InvalidOperationException("Tourist has already reviewed this tour.");

            var review = reviewDTO.ToDomain();
            _reviewRepository.AddReview(review);
            EnrichReview(review);
            return new TourReviewDTO(review);
        }

        public TourReviewDTO UpdateReview(TourReviewDTO reviewDTO)
        {
            if (reviewDTO == null)
                throw new ArgumentNullException(nameof(reviewDTO));

            var review = reviewDTO.ToDomain();
            _reviewRepository.UpdateReview(review);
            EnrichReview(review);
            return new TourReviewDTO(review);
        }

        public void DeleteReview(int reviewId)
        {
            _reviewRepository.DeleteReview(reviewId);
        }

        public double GetAverageRatingForTour(int tourId)
        {
            return _reviewRepository.GetAverageRatingForTour(tourId);
        }

        public bool HasReview(int touristId, int tourId)
        {
            return _reviewRepository.HasReview(touristId, tourId);
        }

        public bool HasTouristReviewedTour(int tourId, int touristId)
        {
            
            return _reviewRepository.GetAll()
                                    .Any(r => r.TourId == tourId && r.TouristId == touristId && r.IsValid);
        }

        // Helper methods
        private List<TourReviewDTO> EnrichReviews(List<TourReview> reviews)
        {
            foreach (var review in reviews)
                EnrichReview(review);

            return reviews.Select(r => new TourReviewDTO(r)).ToList();
        }

        private void EnrichReview(TourReview review)
        {
            if (review.TourId > 0)
            {
                review.Tour = _tourRepository.GetById(review.TourId);
            }

            if (review.TouristId > 0)
            {
                review.Tourist = _userRepository.GetById(review.TouristId);
            }
        }
    }
}
