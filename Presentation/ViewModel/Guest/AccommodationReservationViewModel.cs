using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Utilities;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationReservationViewModel : ViewModelBase
    {
        private readonly AccommodationDetailsDTO _accommodationDetails;
        private readonly IReservationService _reservationService;

        public Action CloseAction { get; set; }

        #region Svojstva za povezivanje (Binding)



        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        private string _guestsNumber;
        public string GuestsNumber
        {
            get => _guestsNumber;
            set { _guestsNumber = value; OnPropertyChanged(); }
        }

        public List<DateTime> OccupiedDates { get; set; }

        #endregion

        #region Komande
        public ICommand ReserveCommand { get; }
        #endregion

        public AccommodationReservationViewModel(AccommodationDetailsDTO accommodationDetails)
        {
            _accommodationDetails = accommodationDetails ?? throw new ArgumentNullException(nameof(accommodationDetails));

            _reservationService = Injector.CreateInstance<IReservationService>();

            ReserveCommand = new RelayCommand(Reserve);

            LoadOccupiedDates();
        }

        #region Logika

        private void LoadOccupiedDates()
        {
            OccupiedDates = _reservationService.GetOccupiedDatesForAccommodation(_accommodationDetails.Id);
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
            try
            {
                var reservationDto = new ReservationDTO
                {
                    AccommodationId = _accommodationDetails.Id,
                    GuestId = Session.CurrentUser.Id,
                    StartDate = StartDate.Value.Date,
                    EndDate = EndDate.Value.Date,
                    GuestsNumber = guestNumber
                };

                _reservationService.Create(reservationDto);

                HandleReservationSuccess();
            }
            catch (Exception ex)
            {
                HandleReservationFailure(ex);
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
        #endregion
    }
}