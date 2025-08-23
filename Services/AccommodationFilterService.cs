using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Services.DTOs;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class AccommodationFilterService : IAccommodationFilterService
    {
        private readonly IAccommodationRepository _accommodationRepository;

        public AccommodationFilterService(IAccommodationRepository accommodationRepository)
        {
            _accommodationRepository = accommodationRepository;
        }

        public List<AccommodationDTO> Filter(AccommodationSearchParameters parameters)
        {
          return _accommodationRepository.GetAll()
                 .Where(acc => MatchesFilter(acc, parameters))
                 .Select(acc => new AccommodationDTO(acc))
                 .ToList();
        }
        //prebaceno iz AccommodationLookup.xaml.cs
        private bool MatchesFilter(Accommodation acc, AccommodationSearchParameters parameters)
        {
            return NameMatches(acc, parameters.Name) &&
                   CountryMatches(acc, parameters.Country) &&
                   CityMatches(acc, parameters.City) &&
                   TypeMatches(acc, parameters.Type) &&
                   MaxGuestsMatches(acc, parameters.MaxGuests) &&
                   MinDaysMatches(acc, parameters.MinDays);
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
