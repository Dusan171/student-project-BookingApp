using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.Services.DTO;
using BookingApp.Domain.Model;

namespace BookingApp.Presentation.ViewModel
{
    public class RescheduleRequestViewModel : ViewModelBase
    {
        private readonly Reservation _reservation;
        private readonly Accommodation _accommodation;
        private readonly IRescheduleRequestService _rescheduleRequestService;
        private readonly IAccommodationService _accommodationService;

        public Action CloseAction { get; set; }

        #region Svojstva za povezivanje (Binding)

        public string AccommodationName => _accommodation?.Name;
        public string CurrentPeriod => $"{_reservation.StartDate:dd.MM.yyyy} - {_reservation.EndDate:dd.MM.yyyy}";

        private DateTime? _newStartDate;
        public DateTime? NewStartDate
        {
            get => _newStartDate;
            set { _newStartDate = value; OnPropertyChanged(); }
        }

        private DateTime? _newEndDate;
        public DateTime? NewEndDate
        {
            get => _newEndDate;
            set { _newEndDate = value; OnPropertyChanged(); }
        }

        public List<DateTime> BlackoutDates { get; set; }

        #endregion

        #region Komande
        public ICommand SendRequestCommand { get; }
        #endregion

        public RescheduleRequestViewModel(Reservation reservation)
        {
            _reservation = reservation ?? throw new ArgumentNullException(nameof(reservation));

            _rescheduleRequestService = Injector.CreateInstance<IRescheduleRequestService>();
            _accommodationService = Injector.CreateInstance<IAccommodationService>();

            _accommodation = _accommodationService.GetAccommodationById(_reservation.AccommodationId).ToAccommodation();
            if (_accommodation == null)
            {
                MessageBox.Show("Could not find accommodation details. The window will close.");
                CloseAction?.Invoke();
                return;
            }

            SendRequestCommand = new RelayCommand(SendRequest);

            LoadInitialData();
        }

        #region Logika

        private void LoadInitialData()
        {
            var reservationDto = new ReservationDTO(_reservation);

            var blackoutDates = _rescheduleRequestService.GetBlackoutDatesForReschedule(reservationDto);

            this.BlackoutDates = blackoutDates;

            OnPropertyChanged(nameof(BlackoutDates));
        }

        private void SendRequest(object obj)
        {
            if (!IsInputValid())
            {
                return;
            }

            try
            {
                var requestDto = new RescheduleRequestDTO
                {
                    ReservationId = _reservation.Id,
                    NewStartDate = NewStartDate.Value.Date,
                    NewEndDate = NewEndDate.Value.Date
                };
                _rescheduleRequestService.CreateRequest(requestDto);

                MessageBox.Show("Your request has been sent to the owner.", "Request Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool IsInputValid()
        {
            if (!NewStartDate.HasValue || !NewEndDate.HasValue)
            {
                MessageBox.Show("Please select both new start and end dates.");
                return false;
            }
            if (NewEndDate.Value <= NewStartDate.Value)
            {
                MessageBox.Show("End date must be after start date.");
                return false;
            }
            return true;
        }

        #endregion
    }
}