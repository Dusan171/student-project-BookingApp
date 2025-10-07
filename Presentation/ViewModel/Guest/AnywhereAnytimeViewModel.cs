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
        private readonly IReservationCreationService _reservationCreationService;

        #region Properties & Commands
        public string GuestsNumber { get; set; }
        public string StayDuration { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ObservableCollection<AnywhereSearchResultDTO> SearchResults { get; set; }
        public ICommand SearchCommand { get; }
        public ICommand ReserveCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        #endregion

        public AnywhereAnytimeViewModel()
        {
            _searchService = Injector.CreateInstance<IAnywhereSearchService>();
            _reservationCreationService = Injector.CreateInstance<IReservationCreationService>();

            SearchCommand = new RelayCommand(Search);
            ReserveCommand = new RelayCommand(Reserve);
            ViewDetailsCommand = new RelayCommand(ViewDetails);

            SearchResults = new ObservableCollection<AnywhereSearchResultDTO>();
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
            if (!TryCreateReservationDTO(selectedOffer, out var reservationDto)) return;

            ExecuteReservation(reservationDto, selectedOffer);
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
                SearchResults.Add(result);
            }
        }

        // Refactoring: "Extract Method" - Validacija i kreiranje DTO za rezervaciju.
        private bool TryCreateReservationDTO(AnywhereSearchResultDTO offer, out ReservationDTO reservationDto)
        {
            reservationDto = null;

            var dateParts = offer.OfferedDateRange.Split(" - ");
            string format = "dd.MM.yyyy";
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (dateParts.Length != 2 ||
                !DateTime.TryParseExact(dateParts[0], format, culture, System.Globalization.DateTimeStyles.None, out var startDate) ||
                !DateTime.TryParseExact(dateParts[1], format, culture, System.Globalization.DateTimeStyles.None, out var endDate))
            {
                MessageBox.Show("An error occurred with the selected date range.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(GuestsNumber, out int guests))
            {
                MessageBox.Show("An error occurred retrieving the number of guests.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            reservationDto = new ReservationDTO
            {
                AccommodationId = offer.Accommodation.Id,
                GuestId = Session.CurrentUser.Id,
                StartDate = startDate,
                EndDate = endDate,
                GuestsNumber = guests
            };
            return true;
        }

        private void ExecuteReservation(ReservationDTO reservationDto, AnywhereSearchResultDTO offer)
        {
            try
            {
                var result = _reservationCreationService.AttemptReservation(reservationDto);
                if (result.IsSuccess)
                {
                    MessageBox.Show($"Reservation for '{offer.Accommodation.Name}' from {reservationDto.StartDate:dd.MM.yyyy} to {reservationDto.EndDate:dd.MM.yyyy} has been successfully made!", "Reservation Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
                    SearchResults.Clear();
                }
                else
                {
                    MessageBox.Show($"Could not make reservation. Reason: {result.ErrorMessage}", "Reservation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}