using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class HomeStatisticsService : IHomeStatisticsService
    {
        private readonly IAccommodationReviewService _accommodationReviewService;
        private readonly IAccommodationService _accommodationService;
        private readonly IReservationService _reservationService;
        public HomeStatisticsService(IAccommodationService accommodationService,
                                     IAccommodationReviewService accommodationReviewService,
                                     IReservationService reservationService)
        {
            _accommodationService = accommodationService;
            _accommodationReviewService = accommodationReviewService;
            _reservationService = reservationService;
           
        }

        public HomeStatisticDTO GetOwnerStatistics(int ownerId)
        {
            var ownersAccommodations = _accommodationService.GetAccommodationsByOwnerId(ownerId);
            var accommodationIds = ownersAccommodations.Select(a => a.Id).ToList();

            var allReservations = _reservationService.GetAll();
            var ownersReservations = allReservations
                                     .Where(r => accommodationIds.Contains(r.AccommodationId))
                                     .ToList();
            var reservationIds = ownersReservations.Select(r => r.Id).ToList();

            var allReviews = _accommodationReviewService.GetAll();
            var ownersReviews = allReviews
                                .Where(r => reservationIds.Contains(r.ReservationId))
                                .ToList();

            double averageGrade = 0;
            if (ownersReviews.Any())
            {
                averageGrade = ownersReviews.Average(r => (r.CleanlinessRating + r.OwnerRating) / 2.0);
            }

            return new HomeStatisticDTO
            {
                TotalReviews = ownersReviews.Count,
                AverageGrade = averageGrade,
                ActiveProperties = ownersAccommodations.Count,
                WelcomeMessage = $"Welcome!"
            };
        }
    }
}