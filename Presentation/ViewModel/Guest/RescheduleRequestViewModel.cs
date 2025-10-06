using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class RescheduleRequestViewModel : ViewModelBase
    {
        private readonly ReservationDetailsDTO _reservationDetails;
        private readonly IRescheduleRequestService _rescheduleRequestService;

        public Action CloseAction { get; set; }

        #region Svojstva za povezivanje (Binding)

        public string AccommodationName => _reservationDetails?.AccommodationName;
        public string CurrentPeriod => $"{_reservationDetails?.StartDate:dd.MM.yyyy} - {_reservationDetails?.EndDate:dd.MM.yyyy}";

        private DateTime? _newStartDate;
        public DateTime? NewStartDate
        {
            get => _newStartDate;
            set { _newStartDate = value; OnPropertyChanged(nameof(NewStayDuration)); }
        }

        private DateTime? _newEndDate;
        public DateTime? NewEndDate
        {
            get => _newEndDate;
            set { _newEndDate = value; OnPropertyChanged(nameof(NewStayDuration)); }
        }

        public int NewStayDuration
        {
            get
            {
                if (NewStartDate.HasValue && NewEndDate.HasValue && NewEndDate.Value > NewStartDate.Value)
                {
                    return (NewEndDate.Value - NewStartDate.Value).Days;
                }
                return 0;
            }
        }
        #endregion

        public ICommand SendRequestCommand { get; }

        public RescheduleRequestViewModel(ReservationDetailsDTO reservationDetails)
        {
            _reservationDetails = reservationDetails ?? throw new ArgumentNullException(nameof(reservationDetails));
            _rescheduleRequestService = Injector.CreateInstance<IRescheduleRequestService>();

            SendRequestCommand = new RelayCommand(SendRequest);

           // LoadInitialData();
        }

        #region Logika

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
                    ReservationId = _reservationDetails.ReservationId,
                    GuestId = _reservationDetails.OriginalReservation.GuestId,
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
                MessageBox.Show("End date must be after the start date.");
                return false;
            }
            return true;
        }

        #endregion
    }
}