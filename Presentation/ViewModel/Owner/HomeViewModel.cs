using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Utilities;
using BookingApp.Presentation.View.Owner;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly IReservationService _reservationService; 
        private readonly IGuestReviewService _guestReviewService; 

        private object _currentContent;
        public object CurrentContent
        {
            get => _currentContent;
            set
            {
                _currentContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowUnratedGuestsCommand { get; }
        public ICommand ShowMyUnitsCommand { get; }
        public ICommand ShowForumsCommand { get; }

        public HomeViewModel(IReservationService reservationService, IGuestReviewService guestReviewService)
        {
            _reservationService = reservationService;
            _guestReviewService = guestReviewService;

            ShowUnratedGuestsCommand = new RelayCommand(ShowUnratedGuests);
            ShowMyUnitsCommand = new RelayCommand(ShowMyUnits);
            ShowForumsCommand = new RelayCommand(ShowForums);

            CurrentContent = new WelcomeView();
        }

        private void ShowUnratedGuests()
        {
            var unratedViewModel = Injector.CreateUnratedGuestsViewModel();

            var unratedView = new UnratedGuestsView();

            unratedView.DataContext = unratedViewModel;

            CurrentContent = unratedView;
        }

        private void ShowMyUnits()
        {
        }

        private void ShowForums()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}