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
        private readonly IAccommodationRepository _accommodationRepository;
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
        public ObservableCollection<Accommodation> Accommodations { get; set; }

        // Svojstvo za praćenje selektovanog smeštaja u DataGrid-u
        private Accommodation _selectedAccommodation;
        public Accommodation SelectedAccommodation
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
            _accommodationRepository = Injector.CreateInstance<IAccommodationRepository>();
            _filterService = Injector.CreateInstance<IAccommodationFilterService>();

            // Inicijalizacija komandi
            SearchCommand = new RelayCommand(Search);
            ReserveCommand = new RelayCommand(Reserve, CanReserve);
            LogoutCommand = new RelayCommand(Logout);
            ResetSearchCommand = new RelayCommand(ResetSearch);

            // Učitavanje početnih podataka
            Accommodations = new ObservableCollection<Accommodation>(_accommodationRepository.GetAll());
        }

        #region Logika Komandi

        private void Search(object obj)
        {
            var searchParams = new AccommodationSearchParameters
            {
                Name = NameSearch,
                Country = CountrySearch,
                City = CitySearch,
                Type = TypeSearch,
                MaxGuests = int.TryParse(MaxGuestsSearch, out int guests) ? guests : 0,
                MinDays = int.TryParse(MinDaysSearch, out int days) ? days : 0
            };

            var result = _filterService.Filter(searchParams);
            Accommodations.Clear();
            foreach (var item in result)
            {
                Accommodations.Add(item);
            }
        }

        private void ResetSearch(object obj)
        {
            // Resetujemo polja za pretragu
            NameSearch = string.Empty;
            CountrySearch = string.Empty;
            CitySearch = string.Empty;
            TypeSearch = null; // Ili "All" ako je to podrazumevana vrednost
            MaxGuestsSearch = string.Empty;
            MinDaysSearch = string.Empty;

            // Ponovo učitavamo sve smeštaje
            var result = _accommodationRepository.GetAll();
            Accommodations.Clear();
            foreach (var item in result)
            {
                Accommodations.Add(item);
            }
        }

        private void Reserve(object obj)
        {
            var reservationWindow = new AccommodationReservationView(SelectedAccommodation);
            reservationWindow.ShowDialog();
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
    }
}