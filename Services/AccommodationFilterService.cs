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
            var allAccommodations = _accommodationRepository.GetAll();
            var filteredAccommodations = allAccommodations.Where(acc => MatchesFilter(acc, parameters));

            //prevod(mapiranje) filtriranih domenskih modela u DTO
            var dtoList = filteredAccommodations.Select(acc => new AccommodationDTO(acc)).ToList();
            return dtoList;
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

        private bool NameMatches(Accommodation acc, string name) =>
            string.IsNullOrEmpty(name) || acc.Name.Contains(name, StringComparison.OrdinalIgnoreCase);

        private bool CountryMatches(Accommodation acc, string country) =>
            string.IsNullOrEmpty(country) || acc.GeoLocation.Country.Contains(country, StringComparison.OrdinalIgnoreCase);

        private bool CityMatches(Accommodation acc, string city) =>
            string.IsNullOrEmpty(city) || acc.GeoLocation.City.Contains(city, StringComparison.OrdinalIgnoreCase);

        private bool TypeMatches(Accommodation acc, string type) =>
            string.IsNullOrEmpty(type) || type == "All" || acc.Type.ToString() == type;

        private bool MaxGuestsMatches(Accommodation acc, int maxGuests) =>
            maxGuests == 0 || acc.MaxGuests <= maxGuests; 

        private bool MinDaysMatches(Accommodation acc, int minDays) =>
            minDays == 0 || acc.MinReservationDays >= minDays; 
    }
}
