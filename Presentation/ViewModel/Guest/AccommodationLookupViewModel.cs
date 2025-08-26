using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Guest; 
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.View;

namespace BookingApp.Presentation.ViewModel
{
    public class AccommodationLookupViewModel : ViewModelBase
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationFilterService _filterService;

        #region Svojstva za povezivanje (Binding)

        private string _nameSearch;
        public string NameSearch
        {
            get => _nameSearch;
            set { _nameSearch = value; OnPropertyChanged(); }
        }

        private string _countrySearch;
        public string CountrySearch
        {
            get => _countrySearch;
            set { _countrySearch = value; OnPropertyChanged(); }
        }

        private string _citySearch;
        public string CitySearch
        {
            get => _citySearch;
            set { _citySearch = value; OnPropertyChanged(); }
        }

        private string _typeSearch;
        public string TypeSearch
        {
            get => _typeSearch;
            set { _typeSearch = value; OnPropertyChanged(); }
        }

        private string _maxGuestsSearch;
        public string MaxGuestsSearch
        {
            get => _maxGuestsSearch;
            set { _maxGuestsSearch = value; OnPropertyChanged(); }
        }

        private string _minDaysSearch;
        public string MinDaysSearch
        {
            get => _minDaysSearch;
            set { _minDaysSearch = value; OnPropertyChanged(); }
        }

        public ObservableCollection<AccommodationDTO> Accommodations { get; set; }

        private AccommodationDTO _selectedAccommodation;
        public AccommodationDTO SelectedAccommodation
        {
            get => _selectedAccommodation;
            set { _selectedAccommodation = value; OnPropertyChanged(); }
        }

        #endregion

        #region Komande (Commands)
        public ICommand SearchCommand { get; }
        public ICommand ReserveCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ResetSearchCommand { get; } 
        #endregion

        public AccommodationLookupViewModel()
        {
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _filterService = Injector.CreateInstance<IAccommodationFilterService>();

            SearchCommand = new RelayCommand(Search);
            ReserveCommand = new RelayCommand(Reserve, CanReserve);
            LogoutCommand = new RelayCommand(Logout);
            ResetSearchCommand = new RelayCommand(ResetSearch);

            Accommodations = new ObservableCollection<AccommodationDTO>();
            InitializeAccommodations();
        }

        #region Logika Komandi

        private void Search(object obj)
        {
            var searchParams = CreateSearchParameters(); 
            var result = _filterService.Filter(searchParams);
            UpdateAccommodations(result);
        }

        private void ResetSearch(object obj)
        {
            ClearSearchFields(); 
            InitializeAccommodations();
        }

        private void Reserve(object obj)
        {
            if (SelectedAccommodation == null) return;

            var fullAccommodation = _accommodationService.GetAccommodationById(SelectedAccommodation.Id);

            if (fullAccommodation != null)
            {
                var reservationWindow = new AccommodationReservationView(fullAccommodation.ToAccommodation());
                reservationWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Could not find details for the selected accommodation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanReserve(object obj)
        {
            return SelectedAccommodation != null;
        }

        private void Logout(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Session.CurrentUser = null;
                var signInWindow = new SignInForm();
                signInWindow.Show();

                Application.Current.Windows.OfType<AccommodationLookup>().FirstOrDefault()?.Close();
            }
        }
        #endregion
        #region Pomocne (privatne) metode - Ciste Funkcije
        private void InitializeAccommodations()
        {
            var allAccommodations = _filterService.Filter(new AccommodationSearchParameters());
            UpdateAccommodations(allAccommodations);
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
        private void UpdateAccommodations(List<AccommodationDTO> accommodations)
        {
            Accommodations.Clear();
            foreach (var accommodation in accommodations)
            {
                Accommodations.Add(accommodation);
            }
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
        #endregion
    }
}