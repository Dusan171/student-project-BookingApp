using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Utilities;
using BookingApp.Presentation.View.Owner;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly IHomeStatisticsService _statisticsService;
        private readonly IUserService _userService;

        private HomeStatisticDTO _statistics;

        public HomeStatisticDTO Statistics
        {
            get => _statistics;
            private set
            {
                _statistics = value;
                OnPropertyChanged();
            }
        }

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
        public ICommand NavigateCommand { get; set; }
        public ICommand ViewSuggestionsCommand { get; } 

        public HomeViewModel(IReservationService reservationService,
                                     IGuestReviewService guestReviewService,
                                     IHomeStatisticsService statisticsService,
                                     IUserService userService)
        {
            _statisticsService = statisticsService;
            _userService = userService;

            ShowUnratedGuestsCommand = new RelayCommand(ShowUnratedGuests);
            ViewSuggestionsCommand = new RelayCommand(() =>
            {
                NavigateCommand?.Execute("Suggestions");
            });
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                int currentOwnerId = _userService.GetCurrentUserId();
                Statistics = _statisticsService.GetOwnerStatistics(currentOwnerId);
            }
            catch
            {
                Statistics = new HomeStatisticDTO
                {
                    TotalReviews = 0,
                    AverageGrade = 0.0,
                    ActiveProperties = 0,
                    WelcomeMessage = "Welcome!"
                };
            }
        }

        private void ShowUnratedGuests()
        {
            NavigateCommand?.Execute("UnratedGuests");
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}