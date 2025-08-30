using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourReviewService
    {
        List<TourReviewDTO> GetAllReviews();
        TourReviewDTO? GetReviewById(int id);
        List<TourReviewDTO> GetReviewsByTour(int tourId);
        List<TourReviewDTO> GetReviewsByTourist(int touristId);
        TourReviewDTO AddReview(TourReviewDTO reviewDTO);
        TourReviewDTO UpdateReview(TourReviewDTO reviewDTO);
        void DeleteReview(int reviewId);
        double GetAverageRatingForTour(int tourId);
        bool HasReview(int touristId, int tourId);

        bool HasTouristReviewedTour(int tourId, int touristId);
   
    }
}