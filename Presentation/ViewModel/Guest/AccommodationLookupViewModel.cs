using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationLookupViewModel : ViewModelBase
    {
        private IAccommodationService _accommodationService;
        public static event Action<AccommodationDetailsDTO> ViewDetailsRequested;
        public static event Action LogoutRequested;
        public event Action<AccommodationDetailsDTO> ReserveRequested;
        public AccommodationSearchViewModel SearchViewModel { get; private set; }
        public ObservableCollection<AccommodationDetailsDTO> Accommodations { get; set; }
        public ICommand ReserveCommand { get; private set; }
        public ICommand ViewDetailsCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        public AccommodationLookupViewModel()
        {
            Accommodations = new ObservableCollection<AccommodationDetailsDTO>();
        }

        public void Initialize()
        {
            _accommodationService = Injector.CreateInstance<IAccommodationService>();
            var filterService = Injector.CreateInstance<IAccommodationFilterService>();

            SearchViewModel = new AccommodationSearchViewModel(filterService);
            SearchViewModel.SearchCompleted += OnSearchCompleted;

            ReserveCommand = new RelayCommand(Reserve);
            LogoutCommand = new RelayCommand(Logout);
            ViewDetailsCommand = new RelayCommand(ViewDetails);

            SearchViewModel.ResetSearchCommand.Execute(null);

            OnPropertyChanged(nameof(SearchViewModel));
        }

        #region Logika Komandi
        private void ViewDetails(object parameter)
        {
            if (parameter is AccommodationDetailsDTO selectedDto)
            {
                ViewDetailsRequested?.Invoke(selectedDto);
            }
        }
        private void Reserve(object parameter)
        {
            if (parameter is AccommodationDetailsDTO selectedDto)
            {
                ReserveRequested?.Invoke(selectedDto);
            }
        }
        private void Logout(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LogoutRequested?.Invoke();
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