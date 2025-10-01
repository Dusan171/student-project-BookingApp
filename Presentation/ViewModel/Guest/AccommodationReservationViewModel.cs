using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Guest;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationReservationViewModel : ViewModelBase
    {
        private readonly AccommodationDetailsDTO _accommodationDetails;
        public AccommodationDetailsDTO AccommodationDetails => _accommodationDetails;
        private readonly IReservationCreationService _reservationCreationService;

        public Action CloseAction { get; set; }

        #region Svojstva za povezivanje (Binding)



        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(nameof(StayDuration)); }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(nameof(StayDuration)); }
        }

        private string _guestsNumber;
        public string GuestsNumber
        {
            get => _guestsNumber;
            set { _guestsNumber = value; OnPropertyChanged(); }
        }
        public int StayDuration
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue && EndDate.Value > StartDate.Value)
                {
                    return (EndDate.Value - StartDate.Value).Days;
                }
                return 0;
            }
        }

        public List<DateTime> OccupiedDates { get; private set; }

        #endregion

        #region Komande
        public ICommand ReserveCommand { get; }
        #endregion

        public AccommodationReservationViewModel(AccommodationDetailsDTO accommodationDetails)
        {
            _accommodationDetails = accommodationDetails ?? throw new ArgumentNullException(nameof(accommodationDetails));

            _reservationCreationService = Injector.CreateInstance<IReservationCreationService>();

            ReserveCommand = new RelayCommand(Reserve);

            LoadOccupiedDates();
        }

        #region Logika

        private void LoadOccupiedDates()
        {
            OccupiedDates = _reservationCreationService.GetOccupiedDatesForAccommodation(_accommodationDetails.Id);
        }
        private void Reserve(object obj)
        {
            if (!IsInputValid(out int guestNumber))
            {
                return;
            }
            ExecuteReservation(guestNumber);
        }
        private bool IsInputValid(out int parsedGuestNumber)
        {
            parsedGuestNumber = 0;

            if (!AreDatesValid())
            {
                return false;
            }

            if (!IsGuestNumberValid(out parsedGuestNumber))
            {
                return false;
            }

            return true;
        }
        private bool AreDatesValid()
        {
            if (!StartDate.HasValue)
            {
                MessageBox.Show("Please select a start date.");
                return false;
            }
            if (!EndDate.HasValue)
            {
                MessageBox.Show("Please select an end date.");
                return false;
            }
            if (EndDate.Value <= StartDate.Value)
            {
                MessageBox.Show("End date must be after the start date.");
                return false;
            }
            return true;
        }
        private bool IsGuestNumberValid(out int parsedGuestNumber)
        {
            if (!int.TryParse(GuestsNumber, out parsedGuestNumber) || parsedGuestNumber <= 0)
            {
                MessageBox.Show("Please enter a valid number of guests.");
                return false;
            }
            return true;
        }
        private void ExecuteReservation(int guestNumber)
        {
            var reservationDto = new ReservationDTO
            {
                AccommodationId = _accommodationDetails.Id,
                GuestId = Session.CurrentUser.Id,
                StartDate = StartDate.Value.Date,
                EndDate = EndDate.Value.Date,
                GuestsNumber = guestNumber
            };

            var result = _reservationCreationService.AttemptReservation(reservationDto);

            if (result.IsSuccess)
            {
                HandleReservationSuccess();
            }
            else
            {
                if (result.SuggestedRanges.Any())
                {
                    HandleUnavailableDates(result.SuggestedRanges, guestNumber);
                }
                else
                {
                    HandleReservationFailure(new Exception(result.ErrorMessage));
                }
            }
        }
        private void HandleReservationSuccess()
        {
            MessageBox.Show("Reservation successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseAction?.Invoke();
        }
        private void HandleReservationFailure(Exception ex)
        {
            MessageBox.Show(ex.Message, "Reservation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void HandleUnavailableDates(List<DateRange> suggestions, int guestNumber)
        {
            var suggestionsWindow = new SuggestedDatesView(suggestions);
            suggestionsWindow.ShowDialog();

            var suggestionsViewModel = (SuggestedDatesViewModel)suggestionsWindow.DataContext;
            if (suggestionsViewModel.IsConfirmed)
            {
                var chosenRange = suggestionsViewModel.SelectedDateRange;

                this.StartDate = chosenRange.StartDate;
                this.EndDate = chosenRange.EndDate;

                MessageBox.Show(
                    $"New dates ({chosenRange.StartDate:dd.MM.yyyy} - {chosenRange.EndDate:dd.MM.yyyy}) have been selected. Please click 'Reserve' again to confirm.",
                    "Dates Updated",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

            }
        }
        #endregion
    }
}