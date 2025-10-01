using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Domain.Interfaces; 
using BookingApp.Services;        
using BookingApp.Services.DTO;    
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AnywhereAnytimeViewModel : ViewModelBase
    {
        private readonly IAccommodationFilterService _filterService;

        public string GuestsNumber { get; set; }
        public string StayDuration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ObservableCollection<AccommodationDetailsDTO> SearchResults { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand ReserveCommand { get; }

        public AnywhereAnytimeViewModel()
        {
            _filterService = Injector.CreateInstance<IAccommodationFilterService>();

            SearchCommand = new RelayCommand(Search);
            ReserveCommand = new RelayCommand(p => { /* Ne radi ništa za sada */ });

            SearchResults = new ObservableCollection<AccommodationDetailsDTO>();

            LoadAllAccommodations();
        }

        private void LoadAllAccommodations()
        {
            SearchResults.Clear();
            var emptyParams = new AccommodationSearchParameters();
            var allAccommodations = _filterService.Filter(emptyParams);

            foreach (var accommodationDto in allAccommodations)
            {
                SearchResults.Add(accommodationDto);
            }
        }

        private void Search(object obj)
        {
            SearchResults.Clear();

            var fake1 = new AccommodationDetailsDTO
            {
                Name = "LAŽNI REZULTAT: Hotel 'Budućnost'",
                GeoLocation = new LocationDTO { City = "Beograd", Country = "Srbija" },
                MaxGuests = 4,
                MinReservationDays = 3,
                Type = "APARTMENT"
            };

            var fake2 = new AccommodationDetailsDTO
            {
                Name = "LAŽNI REZULTAT: Vikendica 'Mir'",
                GeoLocation = new LocationDTO { City = "Novi Sad", Country = "Srbija" },
                MaxGuests = 2,
                MinReservationDays = 5,
                Type = "COTTAGE"
            };

            SearchResults.Add(fake1);
            SearchResults.Add(fake2);
        }
    }
}