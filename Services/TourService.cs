using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;

namespace BookingApp.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITourReservationRepository _reservationRepository;

        public TourService(ITourRepository tourRepository,
                          ILocationRepository locationRepository,
                          IUserRepository userRepository,
                          ITourReservationRepository reservationRepository)
        {
            _tourRepository = tourRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _reservationRepository = reservationRepository;
        }

        // ALTERNATIVNI konstruktor za kompatibilnost
        public TourService(ITourRepository tourRepository,
                          ILocationRepository locationRepository,
                          IUserRepository userRepository)
        {
            _tourRepository = tourRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
            _reservationRepository = Services.Injector.CreateInstance<ITourReservationRepository>();
        }

        public List<Tour> GetAvailableTours()
        {
            var tours = _tourRepository.GetAvailableTours();
            EnrichToursWithDetails(tours);
            return tours;
        }

        public List<Tour> SearchTours(SearchCriteriaDTO criteria)
        {
            if (criteria == null)
                return new List<Tour>();

            var tours = _tourRepository.GetAll();
            EnrichToursWithDetails(tours);

            var query = tours.AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.City))
            {
                query = query.Where(t => t.Location != null &&
                    !string.IsNullOrEmpty(t.Location.City) &&
                    t.Location.City.Contains(criteria.City, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Country))
            {
                query = query.Where(t => t.Location != null &&
                    !string.IsNullOrEmpty(t.Location.Country) &&
                    t.Location.Country.Contains(criteria.Country, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Language))
            {
                query = query.Where(t => !string.IsNullOrEmpty(t.Language) &&
                    t.Language.Equals(criteria.Language, StringComparison.OrdinalIgnoreCase));
            }

            if (criteria.MaxPeople.HasValue)
            {
                query = query.Where(t => GetAvailableSpots(t.Id) >= criteria.MaxPeople.Value);
            }

            if (criteria.Duration.HasValue)
            {
                query = query.Where(t => Math.Abs(t.DurationHours - criteria.Duration.Value) < 0.1);
            }

            try
            {
                return query.ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Greška u pretrazi: {ex.Message}");
                return new List<Tour>();
            }
        }

        public List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots)
        {
            var tours = _tourRepository.GetAll();

            var originalTour = tours.FirstOrDefault(t => t.Id == originalTourId);
            if (originalTour?.Location == null)
                return new List<Tour>();

            var alternatives = tours.Where(t =>
                t.Id != originalTourId &&
                t.Location != null &&
                t.Location.Id == originalTour.Location.Id &&
                GetAvailableSpots(t.Id) >= requiredSpots
            ).ToList();

            return alternatives;
        }

        public Tour? GetTourById(int id)
        {
            return _tourRepository.GetById(id);
        }

        public Tour? GetTourWithDetails(int id)
        {
            var tour = _tourRepository.GetById(id);
            if (tour != null)
            {
                EnrichTourWithDetails(tour);
            }
            return tour;
        }

        public bool ReserveSpots(int tourId, int numberOfSpots)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour != null && GetAvailableSpots(tourId) >= numberOfSpots)
            {
                int newReservedSpots = tour.ReservedSpots + numberOfSpots;
                return _tourRepository.UpdateReservedSpots(tourId, newReservedSpots);
            }
            return false;
        }

        public int GetAvailableSpots(int tourId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null) return 0;


            if (_reservationRepository != null)
            {
                var activeReservationsCount = _reservationRepository.GetByTourId(tourId)
                    .Where(r => r.Status == TourReservationStatus.ACTIVE)
                    .Sum(r => r.NumberOfGuests);

                int availableFromReservations = tour.MaxTourists - activeReservationsCount;

                System.Diagnostics.Debug.WriteLine($"TourService.GetAvailableSpots({tourId}):");
                System.Diagnostics.Debug.WriteLine($"  Tour: {tour.Name}");
                System.Diagnostics.Debug.WriteLine($"  MaxTourists: {tour.MaxTourists}");
                System.Diagnostics.Debug.WriteLine($"  ReservedSpots (CSV): {tour.ReservedSpots}");
                System.Diagnostics.Debug.WriteLine($"  ActiveReservations: {activeReservationsCount}");
                System.Diagnostics.Debug.WriteLine($"  Available (real-time): {availableFromReservations}");

                return Math.Max(0, availableFromReservations);
            }
            else
            {

                int available = tour.MaxTourists - tour.ReservedSpots;
                System.Diagnostics.Debug.WriteLine($"TourService.GetAvailableSpots({tourId}) - Fallback:");
                System.Diagnostics.Debug.WriteLine($"  Available (CSV): {available}");
                return available < 0 ? 0 : available;
            }
        }

        public void EnrichToursWithDetails(List<Tour> tours)
        {
            foreach (var tour in tours)
            {
                EnrichTourWithDetails(tour);
            }
        }

        private void EnrichTourWithDetails(Tour tour)
        {

            if (tour.Location != null && tour.Location.Id > 0)
            {
                var fullLocation = _locationRepository.GetById(tour.Location.Id);
                if (fullLocation != null)
                {
                    tour.Location = fullLocation;
                }
            }


            if (tour.Guide != null && tour.Guide.Id > 0)
            {
                var fullGuide = _userRepository.GetById(tour.Guide.Id);
                if (fullGuide != null)
                {
                    tour.Guide = fullGuide;
                }
            }
        }

        public bool ValidateTour(Tour tour)
        {
            return tour != null
                && !string.IsNullOrWhiteSpace(tour.Name)
                && tour.Location != null
                && tour.MaxTourists > 0
                && tour.DurationHours > 0;
        }

        public List<Tour> GetAllTours()
        {
            var tours = _tourRepository.GetAll();
            EnrichToursWithDetails(tours);
            return tours;
        }

     
    }
}