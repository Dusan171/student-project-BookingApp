using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System.Collections.Generic;
using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;
using BookingApp.Services.Validation;
using BookingApp.Services.Enhancement;
using BookingApp.Services.Management;
using BookingApp.Services.Search;

namespace BookingApp.Services
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly TourDataValidator _dataValidator;
        private readonly TourDataEnhancer _dataEnhancer;
        private readonly TourAvailabilityHandler _availabilityHandler;
        private readonly TourSearchProcessor _searchProcessor;

        public TourService(ITourRepository tourRepository, ILocationRepository locationRepository, 
            IUserRepository userRepository, ITourReservationRepository reservationRepository)
        {
            _tourRepository = tourRepository;
            _dataValidator = new TourDataValidator();
            _dataEnhancer = new TourDataEnhancer(locationRepository, userRepository);
            _availabilityHandler = new TourAvailabilityHandler(tourRepository, reservationRepository);
            _searchProcessor = new TourSearchProcessor();
        }

        // Alternative constructor for compatibility
        public TourService(ITourRepository tourRepository, ILocationRepository locationRepository, IUserRepository userRepository)
        {
            _tourRepository = tourRepository;
            _dataValidator = new TourDataValidator();
            _dataEnhancer = new TourDataEnhancer(locationRepository, userRepository);
            _availabilityHandler = new TourAvailabilityHandler(tourRepository, Services.Injector.CreateInstance<ITourReservationRepository>());
            _searchProcessor = new TourSearchProcessor();
        }

        // Basic operations
        public List<Tour> GetAvailableTours() => PopulateAndReturn(_tourRepository.GetAvailableTours());
        public List<Tour> GetAllTours() => PopulateAndReturn(_tourRepository.GetAll());
        public Tour? GetTourById(int id) => _tourRepository.GetById(id);
        public Tour? GetTourWithDetails(int id) => PopulateSingle(_tourRepository.GetById(id));

        // Availability management - delegated
        public int GetAvailableSpots(int tourId) => _availabilityHandler.CalculateAvailableSpots(tourId);
        public bool ReserveSpots(int tourId, int numberOfSpots) => _availabilityHandler.BookSpots(tourId, numberOfSpots);

        // Validation - delegated
        public bool ValidateTour(Tour tour) => _dataValidator.IsValidTour(tour);

        // Search operations - delegated
        public List<Tour> SearchTours(SearchCriteriaDTO criteria) => 
            _searchProcessor.ProcessSearchCriteria(PopulateAndReturn(_tourRepository.GetAll()), criteria, GetAvailableSpots);

        public List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots) => 
            _searchProcessor.FindAlternativeTours(_tourRepository.GetAll(), originalTourId, requiredSpots, GetAvailableSpots);

        // Data enrichment helper method
        public void EnrichToursWithDetails(List<Tour> tours) => _dataEnhancer.PopulateToursWithData(tours);

        // Private helper methods
        private List<Tour> PopulateAndReturn(List<Tour> tours) { _dataEnhancer.PopulateToursWithData(tours); return tours; }
        private Tour? PopulateSingle(Tour? tour) { if (tour != null) _dataEnhancer.PopulateTourWithData(tour); return tour; }
    }
}