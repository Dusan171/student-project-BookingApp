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

       public ReservationCreationService( IReservationRepository reservationRepository,IOccupiedDateRepository occupiedDateRepository,IAccommodationRepository accommodationRepository)
        {
            _reservationRepository = reservationRepository;
            _occupiedDateRepository = occupiedDateRepository;
            _accommodationRepository = accommodationRepository;
        }
        public ReservationAttemptResult AttemptReservation(ReservationDTO reservationDto)
        {
            try
            {
                var accommodation = GetAndValidateAccommodation(reservationDto.AccommodationId);
                ValidateReservationRules(accommodation, reservationDto.StartDate, reservationDto.EndDate, reservationDto.GuestsNumber);

                if (!IsDateRangeAvailable(accommodation.Id, reservationDto.StartDate, reservationDto.EndDate))
                {
                    int duration = (reservationDto.EndDate - reservationDto.StartDate).Days;
                    var suggestions = FindAvailableDateRanges(accommodation.Id, duration, reservationDto.StartDate);

                    return new ReservationAttemptResult
                    {
                        IsSuccess = false,
                        SuggestedRanges = suggestions
                    };
                }

                var reservationToSave = reservationDto.ToReservation();
                var savedReservation = _reservationRepository.Save(reservationToSave);
                CreateOccupiedDates(savedReservation);

                return new ReservationAttemptResult
                {
                    IsSuccess = true,
                    CreatedReservation = new ReservationDTO(savedReservation)
                };
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
        private List<DateRange> FindAvailableDateRanges(int accommodationId, int duration, DateTime preferredStartDate)
        {
           // var suggestions = new List<DateRange>();
            var occupiedDates = GetOccupiedDatesForAccommodation(accommodationId).ToHashSet();

            var searchContext = new SuggestionSearchContext
            {
                Suggestions = new List<DateRange>(),
                OccupiedDates = occupiedDates,
                Duration = duration,
                PreferredStartDate = preferredStartDate
            };

            SearchInDirection(searchContext, -1, 3); // -1 smer unazad

            SearchInDirection(searchContext, 1, 6); // 1 smer unapred

            return searchContext.Suggestions.OrderBy(r => r.StartDate).ToList();
        }
        private class SuggestionSearchContext
        {
            public List<DateRange> Suggestions { get; set; }
            public HashSet<DateTime> OccupiedDates { get; set; }
            public int Duration { get; set; }
            public DateTime PreferredStartDate { get; set; }
        }
        private void SearchInDirection(SuggestionSearchContext context, int direction, int maxSuggestions)
        {
            for (int i = 1; i <= 30; i++)
            {
                if (context.Suggestions.Count >= maxSuggestions)
                {
                    break;
                }

                var checkStartDate = context.PreferredStartDate.AddDays(direction * i);
                var checkEndDate = checkStartDate.AddDays(context.Duration);

                if (IsDateRangeAvailable(context.OccupiedDates, checkStartDate, checkEndDate))
                {
                    context.Suggestions.Add(new DateRange { StartDate = checkStartDate, EndDate = checkEndDate });
                }
            }
        }
        private bool IsDateRangeAvailable(int accommodationId, DateTime startDate, DateTime endDate)
        {
            var occupiedDates = GetOccupiedDatesForAccommodation(accommodationId).ToHashSet();
            return IsDateRangeAvailable(occupiedDates, startDate, endDate);
        }
        private bool IsDateRangeAvailable(HashSet<DateTime> occupiedDates, DateTime startDate, DateTime endDate)
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (occupiedDates.Contains(date.Date))
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
        private Accommodation GetAndValidateAccommodation(int accommodationId)
        {
            var accommodation = _accommodationRepository.GetById(accommodationId);
            if (accommodation == null)
            {
                throw new InvalidOperationException("Cannot create reservation for an unknown accommodation.");
            }
            return accommodation;
        }
    }
}