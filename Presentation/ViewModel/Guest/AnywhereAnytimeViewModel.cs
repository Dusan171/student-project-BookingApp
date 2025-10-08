using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows; 
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services; 
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AnywhereAnytimeViewModel : ViewModelBase
    {
        public static event Action<AccommodationDetailsDTO> ViewDetailsFromAnywhereRequested;

        private readonly IAnywhereSearchService _searchService;
        private readonly IReservationOrchestratorService _reservationOrchestrator;

        private string _guestsNumber;
        public string GuestsNumber
        {
            get => _guestsNumber;
            set => SetProperty(ref _guestsNumber, value);
        }

        private string _stayDuration;
        public string StayDuration
        {
            get => _stayDuration;
            set => SetProperty(ref _stayDuration, value);
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public ObservableCollection<AnywhereSearchResultDTO> SearchResults { get; }

        public ICommand SearchCommand { get; }
        public ICommand ReserveCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        public AnywhereAnytimeViewModel()
        {
            _searchService = Injector.CreateInstance<IAnywhereSearchService>();
            _reservationOrchestrator = Injector.CreateInstance<IReservationOrchestratorService>();

            SearchResults = new ObservableCollection<AnywhereSearchResultDTO>();
            SearchCommand = new RelayCommand(Search);
            ReserveCommand = new RelayCommand(Reserve);
            ViewDetailsCommand = new RelayCommand(ViewDetails);
        }

        #region Command Logic

        private void Search(object obj)
        {
            if (!TryGetSearchParameters(out var searchParams)) return;

            var results = _searchService.Search(searchParams);
            UpdateSearchResults(results);
        }

        private void Reserve(object parameter)
        {
            if (parameter is not AnywhereSearchResultDTO selectedOffer) return;

            int currentUserId = Session.CurrentUser.Id;

            var result = _reservationOrchestrator.PrepareAndExecuteReservation(selectedOffer, GuestsNumber, currentUserId);

            if (!string.IsNullOrEmpty(result.Message))
            {
                MessageBox.Show(result.Message, "Reservation Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (result.WasSuccessful)
            {
                SearchResults.Clear();
            }
            else if (result.NewStartDate.HasValue && result.NewEndDate.HasValue)
            {
                StartDate = result.NewStartDate.Value;
                EndDate = result.NewEndDate.Value;
            }
        }

        private void ViewDetails(object parameter)
        {
            if (parameter is AnywhereSearchResultDTO selectedDto)
            {
                ViewDetailsFromAnywhereRequested?.Invoke(selectedDto.Accommodation);
            }
        }

        #endregion

        #region Private Helper Methods

        private bool TryGetSearchParameters(out AnywhereSearchParamsDTO searchParams)
        {
            searchParams = null;
            if (!int.TryParse(GuestsNumber, out int guests) || !int.TryParse(StayDuration, out int duration) || guests <= 0 || duration <= 0)
            {
                MessageBox.Show("Please enter valid positive numbers for guests and stay duration.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            searchParams = new AnywhereSearchParamsDTO
            {
                GuestsNumber = guests,
                StayDuration = duration,
                StartDate = this.StartDate,
                EndDate = this.EndDate
            };
            return true;
        }

        private void UpdateSearchResults(List<AnywhereSearchResultDTO> results)
        {
            SearchResults.Clear();
            if (!results.Any())
            {
                MessageBox.Show("No available accommodations found for the specified criteria.", "No Results", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var result in results)
            {
                if (result.Accommodation.ImagePaths == null || !result.Accommodation.ImagePaths.Any())
                {
                    result.Accommodation.ImagePaths = new List<string>
                    {
                        "/Resources/Images/apartman.jpg",
                        "/Resources/Images/cottage.jpg"
                    };
                }
                SearchResults.Add(result);
            }
        }

        #endregion
    }
}