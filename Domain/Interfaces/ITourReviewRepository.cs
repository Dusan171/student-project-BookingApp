using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourReviewRepository
    {
        List<TourReview> GetAll();
        TourReview? GetById(int id);
        List<TourReview> GetByTourId(int tourId);
        List<TourReview> GetByTouristId(int touristId);
        bool HasReview(int touristId, int tourId);
        void AddReview(TourReview review);        
        void UpdateReview(TourReview review);     
        void DeleteReview(int id);                
        double GetAverageRatingForTour(int tourId);

        bool HasReviewForReservation(int reservationId);
    }
}
