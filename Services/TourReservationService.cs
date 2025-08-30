using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class TourReservationService : ITourReservationService
    {
        private readonly ITourReservationRepository _reservationRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IStartTourTimeRepository _startTourTimeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITourReviewService _tourReviewService;

        public TourReservationService(ITourReservationRepository reservationRepository,
                                    ITourRepository tourRepository,
                                    IStartTourTimeRepository startTourTimeRepository,
                                    IUserRepository userRepository,
                                    ITourReviewService tourReviewService)
        {
            _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
            _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
            _startTourTimeRepository = startTourTimeRepository ?? throw new ArgumentNullException(nameof(startTourTimeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tourReviewService = tourReviewService ?? throw new ArgumentNullException(nameof(tourReviewService));
        }

        // POSTOJEĆE METODE
        public List<TourReservationDTO> GetAllReservations()
        {
            var reservations = _reservationRepository.GetAll();
            return EnrichReservationsAndConvertToDTO(reservations);
        }

        public TourReservationDTO? GetReservationById(int id)
        {
            var reservation = _reservationRepository.GetById(id);
            if (reservation == null) return null;

            EnrichReservationWithDetails(reservation);
            return TourReservationDTO.FromDomain(reservation);
        }

        public TourReservationDTO CreateReservation(TourReservationDTO reservationDTO)
        {
            if (reservationDTO == null)
                throw new ArgumentNullException(nameof(reservationDTO));

            if (!ValidateReservation(reservationDTO))
                throw new ArgumentException("Reservation validation failed");

            var reservation = reservationDTO.ToTourReservation();
            var savedReservation = _reservationRepository.Add(reservation);

            // VAŽNO: Ažuriraj ReservedSpots nakon dodavanja rezervacije
            UpdateTourReservedSpots(savedReservation.TourId);

            EnrichReservationWithDetails(savedReservation);
            return TourReservationDTO.FromDomain(savedReservation);
        }

        public TourReservationDTO UpdateReservation(TourReservationDTO reservationDTO)
        {
            if (reservationDTO == null)
                throw new ArgumentNullException(nameof(reservationDTO));

            var reservation = reservationDTO.ToTourReservation();
            var updatedReservation = _reservationRepository.Update(reservation);

            // VAŽNO: Ažuriraj ReservedSpots nakon ažuriranja
            UpdateTourReservedSpots(updatedReservation.TourId);

            EnrichReservationWithDetails(updatedReservation);
            return TourReservationDTO.FromDomain(updatedReservation);
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _reservationRepository.GetById(reservationId);
            if (reservation != null && CanCancelReservation(reservationId))
            {
                reservation.Status = TourReservationStatus.CANCELLED;
                _reservationRepository.Update(reservation);

                // VAŽNO: Ažuriraj ReservedSpots nakon otkazivanja
                UpdateTourReservedSpots(reservation.TourId);
            }
        }

        public void CompleteReservation(int reservationId)
        {
            var reservation = _reservationRepository.GetById(reservationId);
            if (reservation != null && reservation.Status == TourReservationStatus.ACTIVE)
            {
                reservation.Status = TourReservationStatus.COMPLETED;
                _reservationRepository.Update(reservation);
            }
        }

        public List<TourReservationDTO> GetReservationsByTourist(int touristId)
        {
            var reservations = _reservationRepository.GetByTouristId(touristId);
            return EnrichReservationsAndConvertToDTO(reservations);
        }

        public List<TourReservationDTO> GetReservationsForTour(int tourId)
        {
            var reservations = _reservationRepository.GetByTourId(tourId);
            return EnrichReservationsAndConvertToDTO(reservations);
        }

        public List<TourReservationDTO> GetTodaysReservations()
        {
            var reservations = _reservationRepository.GetTodaysReservations();
            return EnrichReservationsAndConvertToDTO(reservations);
        }

        public List<TourReservationDTO> GetCompletedReservationsByTourist(int touristId)
        {
            var reservations = _reservationRepository.GetCompletedReservationsByTourist(touristId);
            return EnrichReservationsAndConvertToDTO(reservations);
        }

        public List<TourReservationDTO> GetUnreviewedCompletedReservations(int touristId)
        {
            var completedReservations = GetCompletedReservationsByTourist(touristId);

            return completedReservations
                .Where(r => !_tourReviewService.HasTouristReviewedTour(touristId, r.TourId))
                .ToList();
        }

        public bool CanCancelReservation(int reservationId)
        {
            var reservation = _reservationRepository.GetById(reservationId);
            if (reservation == null) return false;

            if (reservation.Status != TourReservationStatus.ACTIVE) return false;

            var startTime = _startTourTimeRepository.GetById(reservation.StartTourTimeId);
            if (startTime == null) return false;

            return startTime.Time > DateTime.Now.AddHours(24);
        }

        public int GetAvailableSpotsForTour(int tourId)
        {

            var tour = _tourRepository.GetById(tourId);
            if (tour == null) return 0;

            int availableSpots = tour.MaxTourists - tour.ReservedSpots;
            return Math.Max(0, availableSpots);
        }

        public List<AlternativeTourDTO> GetAlternativeToursForLocation(int tourId)
        {
            var originalTour = _tourRepository.GetById(tourId);
            if (originalTour?.Location == null)
                return new List<AlternativeTourDTO>();

            var alternativeTours = _tourRepository.GetAll()
                .Where(t => t.Id != tourId &&
                           t.Location?.City?.Equals(originalTour.Location.City, StringComparison.OrdinalIgnoreCase) == true &&
                           t.Location?.Country?.Equals(originalTour.Location.Country, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            var alternativeDTOs = new List<AlternativeTourDTO>();

            foreach (var tour in alternativeTours)
            {
                // NE POZIVAJTE UpdateTourReservedSpots() ovde!
                // Samo izračunajte dostupnost bez menjanja CSV-a
                int availableSpots = GetAvailableSpotsForTour(tour.Id);
                alternativeDTOs.Add(new AlternativeTourDTO(tour));
            }

            return alternativeDTOs
                .OrderByDescending(a => a.AvailableSpots)
                .ThenBy(a => a.Name)
                .ToList();
        }
        public bool IsTourFullyBooked(int tourId)
        {
           
            return GetAvailableSpotsForTour(tourId) <= 0;
        }

        public bool ValidateReservation(TourReservationDTO reservationDTO)
        {
            if (reservationDTO == null) return false;
            if (reservationDTO.NumberOfGuests <= 0) return false;

           
            var availableSpots = GetAvailableSpotsForTour(reservationDTO.TourId);
            return availableSpots >= reservationDTO.NumberOfGuests;
        }
        private void UpdateTourReservedSpots(int tourId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null) return;

            // Izračunaj trenutno rezervisana mesta
            var activeReservations = _reservationRepository.GetByTourId(tourId)
                .Where(r => r.Status == TourReservationStatus.ACTIVE);

            int totalReservedSpots = activeReservations.Sum(r => r.NumberOfGuests);

            // Ažuriraj ReservedSpots u Tour objektu
            tour.ReservedSpots = totalReservedSpots;

            // Sačuvaj promene
            _tourRepository.Update(tour);
        }

        private List<TourReservationDTO> EnrichReservationsAndConvertToDTO(List<TourReservation> reservations)
        {
            foreach (var reservation in reservations)
            {
                EnrichReservationWithDetails(reservation);
            }
            return reservations.Select(r => TourReservationDTO.FromDomain(r)).ToList();
        }

        private void EnrichReservationWithDetails(TourReservation reservation)
        {
            if (reservation.TourId > 0)
            {
                reservation.Tour = _tourRepository.GetById(reservation.TourId);
            }

            if (reservation.StartTourTimeId > 0)
            {
                reservation.StartTourTime = _startTourTimeRepository.GetById(reservation.StartTourTimeId);
            }

            if (reservation.Tour?.Guide?.Id > 0)
            {
                var guide = _userRepository.GetById(reservation.Tour.Guide.Id);
                if (guide != null)
                {
                    reservation.Tour.Guide = guide;
                }
            }
        }

        public List<TourReservationDTO> GetCompletedUnreviewedReservations(int touristId)
        {
            var completedReservations = _reservationRepository.GetCompletedReservationsByTourist(touristId);

            var unreviewedReservations = completedReservations
                .Where(r => !_tourReviewService.HasTouristReviewedTour(touristId, r.TourId))
                .ToList();

            foreach (var reservation in unreviewedReservations)
                EnrichReservationWithDetails(reservation);

            return unreviewedReservations.Select(r => TourReservationDTO.FromDomain(r)).ToList();
        }
    }
}