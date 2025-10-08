using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using System;
using System.Linq;

namespace BookingApp.Services.Management
{
    public class TourAvailabilityHandler
    {
        private readonly ITourRepository _tourRepository;
        private readonly ITourReservationRepository _reservationRepository;

        public TourAvailabilityHandler(ITourRepository tourRepository, ITourReservationRepository reservationRepository)
        {
            _tourRepository = tourRepository;
            _reservationRepository = reservationRepository;
        }

        public int CalculateAvailableSpots(int tourId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null) return 0;

            if (_reservationRepository != null)
            {
                var activeBookings = _reservationRepository.GetByTourId(tourId)
                    .Where(r => r.Status == TourReservationStatus.ACTIVE)
                    .Sum(r => r.NumberOfGuests);

                int availableFromBookings = tour.MaxTourists - activeBookings;
                
                System.Diagnostics.Debug.WriteLine($"TourService.CalculateAvailableSpots({tourId}):");
                System.Diagnostics.Debug.WriteLine($"  Tour: {tour.Name}");
                System.Diagnostics.Debug.WriteLine($"  MaxTourists: {tour.MaxTourists}");
                System.Diagnostics.Debug.WriteLine($"  ReservedSpots (CSV): {tour.ReservedSpots}");
                System.Diagnostics.Debug.WriteLine($"  ActiveBookings: {activeBookings}");
                System.Diagnostics.Debug.WriteLine($"  Available (real-time): {availableFromBookings}");

                return Math.Max(0, availableFromBookings);
            }
            else
            {
                int available = tour.MaxTourists - tour.ReservedSpots;
                System.Diagnostics.Debug.WriteLine($"TourService.CalculateAvailableSpots({tourId}) - Fallback:");
                System.Diagnostics.Debug.WriteLine($"  Available (CSV): {available}");
                return available < 0 ? 0 : available;
            }
        }

        public bool BookSpots(int tourId, int numberOfSpots)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour != null && CalculateAvailableSpots(tourId) >= numberOfSpots)
            {
                int newReservedSpots = tour.ReservedSpots + numberOfSpots;
                return _tourRepository.UpdateReservedSpots(tourId, newReservedSpots);
            }
            return false;
        }
    }
}