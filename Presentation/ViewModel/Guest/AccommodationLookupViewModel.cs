using System.Collections.Generic;
using System.Collections.ObjectModel; // Koristićemo ObservableCollection
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Guest; // Potrebno za otvaranje novog prozora
using BookingApp.Services;
using BookingApp.Services.DTOs;
using BookingApp.Utilities;
using BookingApp.View;

namespace BookingApp.Presentation.ViewModel
{
    public class AccommodationLookupViewModel : ViewModelBase
    {
        // Servisi i repozitorijumi dobijeni preko Injector-a
        private readonly IAccommodationService _accommodationService;
        private readonly IAccommodationFilterService _filterService;

        #region Svojstva za povezivanje (Binding)

        // Svojstva za polja za pretragu
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

        // Kolekcija za prikaz u DataGrid-u. Koristimo ObservableCollection
        // da bi se DataGrid automatski osvežio kada se kolekcija promeni.
        public ObservableCollection<AccommodationDTO> Accommodations { get; set; }

        // Svojstvo za praćenje selektovanog smeštaja u DataGrid-u
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
        public ICommand ResetSearchCommand { get; } // Dodatna komanda za resetovanje pretrage
        #endregion

        public AccommodationLookupViewModel()
        {
            // Dobijanje zavisnosti od Injector-a
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            _filterService = Injector.CreateInstance<IAccommodationFilterService>();

            // Inicijalizacija komandi
            SearchCommand = new RelayCommand(Search);
            ReserveCommand = new RelayCommand(Reserve, CanReserve);
            LogoutCommand = new RelayCommand(Logout);
            ResetSearchCommand = new RelayCommand(ResetSearch);

            // Učitavanje početnih podataka
            Accommodations = new ObservableCollection<AccommodationDTO>();
            InitializeAccommodations();
        }

        #region Logika Komandi

        private void Search(object obj)
        {
            var searchParams = CreateSearchParameters(); // Koristimo novu pomoćnu metodu
            var result = _filterService.Filter(searchParams);
            UpdateAccommodations(result);
        }

        private void ResetSearch(object obj)
        {
            ClearSearchFields(); // Koristimo novu pomoćnu metodu
            InitializeAccommodations();
        }

        private void Reserve(object obj)
        {
            // Selektovana stavka je sada DTO, npr. AccommodationDTO
            if (SelectedAccommodation == null) return;

            // --- PROMENA #5: Dobavljamo pun domenski model na osnovu ID-a iz DTO-a ---
            var fullAccommodation = _accommodationService.GetById(SelectedAccommodation.Id); // Pretpostavka da DTO ima 'Id' i repo ima 'GetById'

            if (fullAccommodation != null)
            {
                // Prosleđujemo pun objekat novom prozoru
                var reservationWindow = new AccommodationReservationView(fullAccommodation);
                reservationWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Could not find details for the selected accommodation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanReserve(object obj)
        {
            // Dugme "Reserve" je omogućeno samo ako je nešto selektovano u DataGrid-u
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

                // Zatvaranje prozora je odgovornost View-a
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