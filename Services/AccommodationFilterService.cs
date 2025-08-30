using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class AccommodationFilterService : IAccommodationFilterService
    {
        private readonly IAccommodationRepository _accommodationRepository;

        public AccommodationFilterService(IAccommodationRepository accommodationRepository)
        {
            _accommodationRepository = accommodationRepository;
        }

        public List<AccommodationDetailsDTO> Filter(AccommodationSearchParameters parameters)
        {
            return _accommodationRepository.GetAll()
                   .Where(acc => MatchesFilter(acc, parameters))
                   .Select(acc => new AccommodationDetailsDTO(acc))
                   .ToList();
        }
        private static bool MatchesFilter(Accommodation accommodation, AccommodationSearchParameters parameters)
        {
            if (accommodation == null || accommodation.GeoLocation == null || parameters == null)
            {
                return false;
            }
            return NameMatches(accommodation, parameters.Name) &&
                   CountryMatches(accommodation, parameters.Country) &&
                   CityMatches(accommodation, parameters.City) &&
                   TypeMatches(accommodation, parameters.Type) &&
                   MaxGuestsMatches(accommodation, parameters.MaxGuests) &&
                   MinDaysMatches(accommodation, parameters.MinDays);
        }

        private static bool NameMatches(Accommodation acc, string name) =>
            string.IsNullOrEmpty(name) || acc.Name.Contains(name, StringComparison.OrdinalIgnoreCase);

        private static bool CountryMatches(Accommodation acc, string country) =>
            string.IsNullOrEmpty(country) || acc.GeoLocation.Country.Contains(country, StringComparison.OrdinalIgnoreCase);

        private static bool CityMatches(Accommodation acc, string city) =>
            string.IsNullOrEmpty(city) || acc.GeoLocation.City.Contains(city, StringComparison.OrdinalIgnoreCase);

        private static bool TypeMatches(Accommodation acc, string type) =>
            string.IsNullOrEmpty(type) || type == "All" || acc.Type.ToString() == type;

        private static bool MaxGuestsMatches(Accommodation acc, int maxGuests) =>
            maxGuests == 0 || acc.MaxGuests <= maxGuests;

        private static bool MinDaysMatches(Accommodation acc, int minDays) =>
            minDays == 0 || acc.MinReservationDays >= minDays;
    }
}