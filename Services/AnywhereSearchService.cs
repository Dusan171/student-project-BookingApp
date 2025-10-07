using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class AnywhereSearchService : IAnywhereSearchService
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IReservationRepository _reservationRepository;

        public AnywhereSearchService(IAccommodationRepository accommodationRepository, IReservationRepository reservationRepository)
        {
            _accommodationRepository = accommodationRepository;
            _reservationRepository = reservationRepository;
        }

        public List<AnywhereSearchResultDTO> Search(AnywhereSearchParamsDTO searchParams)
        {
            var allAccommodations = _accommodationRepository.GetAll();
            var allReservations = _reservationRepository.GetAll();
            var suitableAccommodations = FilterAccommodationsByBasicCriteria(allAccommodations, searchParams);

            return FindAvailableSlots(suitableAccommodations, allReservations, searchParams);
        }

        #region Private Helper Methods

        private List<AnywhereSearchResultDTO> FindAvailableSlots(List<Accommodation> suitableAccommodations, List<Reservation> allReservations, AnywhereSearchParamsDTO searchParams)
        {
            var results = new List<AnywhereSearchResultDTO>();
            var (searchStartDate, searchEndDate) = GetSearchDateRange(searchParams);

            foreach (var accommodation in suitableAccommodations)
            {
                var reservationsForAccommodation = allReservations
                    .Where(r => r.AccommodationId == accommodation.Id && r.Status != ReservationStatus.Cancelled)
                    .ToList();

                var firstAvailableSlot = FindFirstAvailableSlotFor(accommodation, reservationsForAccommodation, searchParams, searchStartDate, searchEndDate);

                if (firstAvailableSlot != null)
                {
                    results.Add(firstAvailableSlot);
                }
            }
            return results;
        }
        private AnywhereSearchResultDTO FindFirstAvailableSlotFor(Accommodation accommodation, List<Reservation> reservations, AnywhereSearchParamsDTO searchParams, DateTime searchStart, DateTime searchEnd)
        {
            for (var day = searchStart; day.AddDays(searchParams.StayDuration) <= searchEnd; day = day.AddDays(1))
            {
                var potentialStartDate = day;
                var potentialEndDate = day.AddDays(searchParams.StayDuration);

                if (IsPeriodAvailable(potentialStartDate, potentialEndDate, reservations))
                {
                    var accommodationDetails = new AccommodationDetailsDTO(accommodation);

                    var resultDto = new AnywhereSearchResultDTO(accommodationDetails)
                    {
                        OfferedDateRange = $"{potentialStartDate:dd.MM.yyyy} - {potentialEndDate:dd.MM.yyyy}"
                    };

                    return resultDto;
                }
            }

            return null;
        }
        private List<Accommodation> FilterAccommodationsByBasicCriteria(List<Accommodation> allAccommodations, AnywhereSearchParamsDTO searchParams)
        {
            return allAccommodations
                .Where(a => a.MaxGuests >= searchParams.GuestsNumber && a.MinReservationDays <= searchParams.StayDuration)
                .ToList();
        }
        private (DateTime, DateTime) GetSearchDateRange(AnywhereSearchParamsDTO searchParams)
        {
            if (searchParams.StartDate.HasValue && searchParams.EndDate.HasValue)
            {
                return (searchParams.StartDate.Value, searchParams.EndDate.Value);
            }
            else
            {
                var defaultStartDate = DateTime.Now.Date.AddDays(1);
                var defaultEndDate = defaultStartDate.AddYears(1);
                return (defaultStartDate, defaultEndDate);
            }
        }
        private bool IsPeriodAvailable(DateTime startDate, DateTime endDate, List<Reservation> reservationsForAccommodation)
        {
            return reservationsForAccommodation.All(existing =>
                startDate >= existing.EndDate || endDate <= existing.StartDate);
        }
        #endregion
    }
}