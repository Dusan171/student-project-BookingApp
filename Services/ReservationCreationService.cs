using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class ReservationCreationService:IReservationCreationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IOccupiedDateRepository _occupiedDateRepository;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly ISuggestionService _suggestionService;

        public ReservationCreationService( IReservationRepository reservationRepository,IOccupiedDateRepository occupiedDateRepository,IAccommodationRepository accommodationRepository, ISuggestionService suggestionService)
        {
            _reservationRepository = reservationRepository;
            _occupiedDateRepository = occupiedDateRepository;
            _accommodationRepository = accommodationRepository;
            _suggestionService = suggestionService;
        }
        public ReservationAttemptResult AttemptReservation(ReservationDTO reservationDto)
        {
            try
            {
                var accommodation = GetAndValidateAccommodation(reservationDto.AccommodationId);
                ValidateReservationRules(accommodation, reservationDto);

                if (IsDateRangeAvailable(accommodation.Id, reservationDto.StartDate, reservationDto.EndDate))
                {
                    return HandleSuccessfulReservation(reservationDto);
                }
                var suggestions = _suggestionService.FindAvailableDateRanges(accommodation.Id, (reservationDto.EndDate - reservationDto.StartDate).Days, reservationDto.StartDate);
                return new ReservationAttemptResult { IsSuccess = false, SuggestedRanges = suggestions };
            }
            catch (Exception ex)
            {
                return new ReservationAttemptResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        public List<DateTime> GetOccupiedDatesForAccommodation(int accommodationId)
        {
            var occupiedDateObjects = _occupiedDateRepository.GetByAccommodationId(accommodationId);
            return occupiedDateObjects.Select(occupiedDate => occupiedDate.Date).ToList();
        }
        private bool IsDateRangeAvailable(int accommodationId, DateTime startDate, DateTime endDate)
        {
            var occupiedDates = _occupiedDateRepository.GetByAccommodationId(accommodationId).Select(o => o.Date.Date).ToHashSet();
            for (DateTime date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
            {
                if (occupiedDates.Contains(date))
                {
                    return false;
                }
            }
            return true;
        }
        private void CreateOccupiedDates(Reservation reservation)
        {
            if (reservation == null) return;

            var occupiedDatesToSave = new List<OccupiedDate>();

            for (DateTime date = reservation.StartDate; date < reservation.EndDate; date = date.AddDays(1))
            {
                occupiedDatesToSave.Add(new OccupiedDate
                {
                    AccommodationId = reservation.AccommodationId,
                    ReservationId = reservation.Id,
                    Date = date.Date
                });
            }

            if (occupiedDatesToSave.Any())
            {
                _occupiedDateRepository.Save(occupiedDatesToSave);
            }
        }
        private void ValidateReservationRules(Accommodation accommodation, ReservationDTO reservationDto)
        {
            if (reservationDto.GuestsNumber > accommodation.MaxGuests)
                throw new Exception($"Max allowed guests: {accommodation.MaxGuests}");

            int stayLength = (reservationDto.EndDate - reservationDto.StartDate).Days;
            if (stayLength < accommodation.MinReservationDays)
                throw new Exception($"Minimum stay is {accommodation.MinReservationDays} days.");
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
        private ReservationAttemptResult HandleSuccessfulReservation(ReservationDTO reservationDto)
        {
            var reservationToSave = reservationDto.ToReservation();
            var savedReservation = _reservationRepository.Save(reservationToSave);
            CreateOccupiedDates(savedReservation);
            return new ReservationAttemptResult { IsSuccess = true, CreatedReservation = new ReservationDTO(savedReservation) };
        }
    }
}