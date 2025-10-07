using System;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.View;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class GuestMainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }
        #region Komande
        public ICommand ShowAccommodationsCommand { get; }
        public ICommand ShowMyReservationsCommand { get; }
        public ICommand ShowForumsCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowAnywhereAnytimeCommand { get; }
        #endregion
        public static event Action CloseMainWindowRequested;
        public static event Action<AccommodationDetailsDTO> OpenReservationWindowRequested;
        public GuestMainViewModel()
        {
            ShowAccommodationsCommand = new RelayCommand(p => ShowAccommodations());
            ShowMyReservationsCommand = new RelayCommand(p => CurrentViewModel = new MyReservationsViewModel());
            ShowForumsCommand = new RelayCommand(p => CurrentViewModel = new ForumListViewModel());
            ShowSettingsCommand = new RelayCommand(p => CurrentViewModel = new SettingsViewModel());
            ShowAnywhereAnytimeCommand = new RelayCommand(p => CurrentViewModel = new AnywhereAnytimeViewModel());
        }
        public void InitializeSubscribers()
        {
            AccommodationLookupViewModel.ViewDetailsRequested += ShowAccommodationDetails;
            AccommodationDetailsViewModel.GoBackToSearchRequested += ShowAccommodations;
            AccommodationDetailsViewModel.ReserveFromDetailsRequested += OnReserveFromDetailsRequested;
            ForumListViewModel.ViewForumRequested += ShowForumDetails;
            AnywhereAnytimeViewModel.ViewDetailsFromAnywhereRequested += ShowAccommodationDetails;

            ShowAccommodations();
        }

        #region Logika Navigacije i Akcija

        private void OnReserveFromDetailsRequested(AccommodationDetailsDTO accommodation)
        {
            OpenReservationWindowRequested?.Invoke(accommodation);
        }
        private void ShowAccommodations() 
        {
            var vm = new AccommodationLookupViewModel();

            vm.Initialize();

            CurrentViewModel = vm;
        }
        private void ShowMyReservations(object obj) => CurrentViewModel = new MyReservationsViewModel();
        private void ShowForums(object obj) => CurrentViewModel = new ForumListViewModel();
        private void ShowSettings(object obj) => CurrentViewModel = new SettingsViewModel();
        private void ShowAnywhereAnytime(object obj) => CurrentViewModel = new AnywhereAnytimeViewModel();
        private void ShowAccommodationDetails(AccommodationDetailsDTO accommodation)
        {
            CurrentViewModel = new AccommodationDetailsViewModel(accommodation);
        }
        public void ShowForumDetails(ForumDTO forum)
        {
            CurrentViewModel = new ForumViewViewModel(forum);
        }
        public void HandleLogout()
        {
            Cleanup(); 
            Session.CurrentUser = null;
            var signInWindow = new SignInForm();
            signInWindow.Show();
            CloseMainWindowRequested?.Invoke();
        }
        public void Cleanup()
        {
            AccommodationLookupViewModel.ViewDetailsRequested -= ShowAccommodationDetails;
            AccommodationDetailsViewModel.GoBackToSearchRequested -= ShowAccommodations;
            AccommodationDetailsViewModel.ReserveFromDetailsRequested -= OnReserveFromDetailsRequested;
            ForumListViewModel.ViewForumRequested -= ShowForumDetails;
            AnywhereAnytimeViewModel.ViewDetailsFromAnywhereRequested -= ShowAccommodationDetails;
        }

        #endregion
    }
}