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

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationLookupViewModel : ViewModelBase
    {
        private readonly IAccommodationService _accommodationService;

        public AccommodationSearchViewModel SearchViewModel { get; }

        public ObservableCollection<AccommodationDetailsDTO> Accommodations { get; set; }

        private AccommodationDetailsDTO _selectedAccommodation;
        public AccommodationDetailsDTO SelectedAccommodation
        {
            get => _selectedAccommodation;
            set { _selectedAccommodation = value; OnPropertyChanged(); }
        }

        public ICommand ReserveCommand { get; }
        public ICommand LogoutCommand { get; }

        public AccommodationLookupViewModel()
        {
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            var filterService = Injector.CreateInstance<IAccommodationFilterService>();

            SearchViewModel = new AccommodationSearchViewModel(filterService);

            SearchViewModel.SearchCompleted += OnSearchCompleted;

            ReserveCommand = new RelayCommand(Reserve, CanReserve);
            LogoutCommand = new RelayCommand(Logout);

            Accommodations = new ObservableCollection<AccommodationDetailsDTO>();

            SearchViewModel.ResetSearchCommand.Execute(null);
        }

        #region Logika Komandi

        private void Reserve(object obj)
        {
            if (SelectedAccommodation == null)
            {
                return;
            }
            var reservationWindow = new AccommodationReservationView(SelectedAccommodation);
            reservationWindow.ShowDialog();
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

        #region Pomoćne metode

        private void OnSearchCompleted(List<AccommodationDetailsDTO> result)
        {
            Accommodations = new ObservableCollection<AccommodationDetailsDTO>(result);
            OnPropertyChanged(nameof(Accommodations));
        }

        #endregion
    }
}