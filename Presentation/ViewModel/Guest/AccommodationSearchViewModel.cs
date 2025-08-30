using System;
using System.Collections.Generic;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationSearchViewModel : ViewModelBase
    {
        private readonly IAccommodationFilterService _filterService;

        public string NameSearch { get; set; }
        public string CountrySearch { get; set; }
        public string CitySearch { get; set; }
        public string TypeSearch { get; set; }
        public string MaxGuestsSearch { get; set; }
        public string MinDaysSearch { get; set; }
 
        public event Action<List<AccommodationDetailsDTO>> SearchCompleted;

        public ICommand SearchCommand { get; }
        public ICommand ResetSearchCommand { get; }

        public AccommodationSearchViewModel(IAccommodationFilterService filterService)
        {
            _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));

            SearchCommand = new RelayCommand(Search);
            ResetSearchCommand = new RelayCommand(ResetSearch);
        }

        private void Search(object obj)
        {
            var searchParams = CreateSearchParameters();
            var result = _filterService.Filter(searchParams);
   
            SearchCompleted?.Invoke(result);
        }

        private void ResetSearch(object obj)
        {
            ClearSearchFields();
   
            Search(null);
        }

        private AccommodationSearchParameters CreateSearchParameters()
        {
            return new AccommodationSearchParameters
            {
                Name = NameSearch,
                Country = CountrySearch,
                City = CitySearch,
                Type = TypeSearch,
                MaxGuests = int.TryParse(MaxGuestsSearch, out int guests) ? guests : 0,
                MinDays = int.TryParse(MinDaysSearch, out int days) ? days : 0
            };
        }

        private void ClearSearchFields()
        {
            NameSearch = string.Empty;
            CountrySearch = string.Empty;
            CitySearch = string.Empty;
            TypeSearch = null;
            MaxGuestsSearch = string.Empty;
            MinDaysSearch = string.Empty;
        }
    }
}