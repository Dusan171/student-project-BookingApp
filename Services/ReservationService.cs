using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IOccupiedDateRepository _occupiedDateRepository;
        private readonly IAccommodationRepository _accommodationRepository;


        public ReservationService(IReservationRepository reservationRepository, IOccupiedDateRepository occupiedDateRepository, IAccommodationRepository accommodationRepository)
        {
            _reservationRepository = reservationRepository;
            _occupiedDateRepository = occupiedDateRepository;
            _accommodationRepository = accommodationRepository;
        }
        
        public ReservationDTO Create(ReservationDTO reservationDto)
        {
            var accommodation = GetAndValidateAccommodation(reservationDto.AccommodationId);

            ValidateReservationRules(accommodation, reservationDto.StartDate, reservationDto.EndDate, reservationDto.GuestsNumber);

            CheckForOverlappingDates(accommodation.Id, reservationDto.StartDate, reservationDto.EndDate);

            var reservationToSave = reservationDto.ToReservation();
            var savedReservation = _reservationRepository.Save(reservationToSave);

            CreateOccupiedDates(accommodation.Id, savedReservation.Id, savedReservation.StartDate, savedReservation.EndDate);

            return new ReservationDTO(savedReservation);
        }
        private Accommodation GetAndValidateAccommodation(int accommodationId)
        {
            var accommodation = _accommodationRepository.GetById(accommodationId);
            if (accommodation == null)
            {
                throw new InvalidOperationException("Cannot create reservation for an unknown accommodation.");
            }
            return accommodation;
        }
        private void ValidateReservationRules(Accommodation accommodation, DateTime startDate, DateTime endDate, int guestNumber)
        {
            if (guestNumber > accommodation.MaxGuests)
            {
                throw new Exception($"Max allowed guests: {accommodation.MaxGuests}");
            }
            int stayLength = (endDate - startDate).Days;
            if (stayLength < accommodation.MinReservationDays)
            {
                throw new Exception($"Minimum stay is {accommodation.MinReservationDays} days.");
            }
        }
        private void CheckForOverlappingDates(int accommodationId, DateTime startDate, DateTime endDate)
        {
            var occupiedDates = _occupiedDateRepository.GetByAccommodationId(accommodationId);
            int stayLength = (endDate - startDate).Days;
            bool isOverlap = Enumerable.Range(0, stayLength)
                .Select(offset => startDate.AddDays(offset).Date)
                .Any(date => occupiedDates.Any(o => o.Date == date));

            if (isOverlap)
            {
                throw new Exception("Selected period is not available.");
            }
        }
        private void CreateOccupiedDates(int accommodationId, int reservationId, DateTime startDate, DateTime endDate)
        {
            List<OccupiedDate> occupiedDatesToSave = new List<OccupiedDate>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                occupiedDatesToSave.Add(new OccupiedDate
                {
                    AccommodationId = accommodationId,
                    ReservationId = reservationId,
                    Date = date
                });
            }
            _occupiedDateRepository.Save(occupiedDatesToSave);
        }

        public List<ReservationDTO> GetAll()
        {
            return _reservationRepository.GetAll()  
                                         .Select(reservation => new ReservationDTO(reservation))  // mapiranje u DTO
                                         .ToList();
        }
        public List<DateTime> GetOccupiedDatesForAccommodation(int accommodationId)
        { 
            
            var occupiedDateObjects = _occupiedDateRepository.GetByAccommodationId(accommodationId);

            
            return occupiedDateObjects.Select(occupiedDate => occupiedDate.Date).ToList();
        }

        public ReservationDTO GetById(int id)
        {
            var reservation = _reservationRepository.GetAll().FirstOrDefault(r => r.Id == id);
            return reservation == null ? null : new ReservationDTO(reservation);
        }

        public void Update(ReservationDTO reservationDto)
        {
            var reservation = reservationDto.ToReservation();
            _reservationRepository.Update(reservation);
        }

        public bool IsAccommodationAvailable(int accommodationId, DateTime newStartDate, DateTime newEndDate)
        {
            var allReservations = _reservationRepository.GetAll();

            var reservationsForAccommodation = allReservations
                .Where(r => r.AccommodationId == accommodationId)
                .ToList();

            foreach (var reservation in reservationsForAccommodation)
            {
                bool datesOverlap = (newStartDate < reservation.EndDate) && (newEndDate > reservation.StartDate);

                if (datesOverlap)
                {
                    return false;
                }
            }
            return true; 
        }

    }
}
