using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class MyReservationsViewModel : ViewModelBase
    {
        private readonly IReservationDisplayService _displayService;
        private readonly IReservationCancellationService _cancellationService;
        private readonly IGuestReviewService _guestReviewService;
        private readonly IReportGenerationService _reportService; 

        public event Action<ReservationDetailsDTO> RescheduleRequested;
        public event Action<ReservationDetailsDTO> RateRequested;

        public static event Action NavigateBackToSearchRequested;

        #region Properties & Commands
        public ObservableCollection<ReservationDetailsDTO> Reservations { get; set; }
        public DateTime? ReportStartDate { get; set; }
        public DateTime? ReportEndDate { get; set; }
        public ICommand RescheduleCommand { get; }
        public ICommand RateCommand { get; }
        public ICommand ViewReviewCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CancelReservationCommand { get; }
        public ICommand GenerateReportCommand { get; }
        #endregion

        public MyReservationsViewModel()
        {
            _displayService = Injector.CreateInstance<IReservationDisplayService>();
            _cancellationService = Injector.CreateInstance<IReservationCancellationService>();
            _guestReviewService = Injector.CreateInstance<IGuestReviewService>();
            _reportService = Injector.CreateInstance<IReportGenerationService>();

            RescheduleCommand = new RelayCommand(p => RescheduleRequested?.Invoke(p as ReservationDetailsDTO));
            RateCommand = new RelayCommand(p => RateRequested?.Invoke(p as ReservationDetailsDTO));
            ViewReviewCommand = new RelayCommand(ViewReview);
            CloseCommand = new RelayCommand(p => NavigateBackToSearchRequested?.Invoke());
            CancelReservationCommand = new RelayCommand(CancelReservation);
            GenerateReportCommand = new RelayCommand(GenerateReport);

            LoadReservations();
        }

        #region Command Logic (sada su metode mnogo kraće)

        private void GenerateReport(object parameter)
        {
            if (!IsReportDataValid(out var data)) return;

            try
            {
                _reportService.GenerateReservationReport(data.FilteredReservations, data.StartDate, data.EndDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Report Generation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelReservation(object parameter)
        {
            if (parameter is not ReservationDetailsDTO selected) return;

            var result = MessageBox.Show($"Are you sure you want to cancel the reservation for {selected.AccommodationName}?", "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _cancellationService.CancelReservation(selected.ReservationId);
                    MessageBox.Show("Reservation successfully cancelled.");
                    LoadReservations();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Cancellation Failed", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void ViewReview(object parameter)
        {
            if (parameter is not ReservationDetailsDTO selected) return;

            var reviewDto = _guestReviewService.GetReviewForReservation(selected.ReservationId);
            if (reviewDto != null)
            {
                var detailsWindow = new GuestReviewDetailsView(reviewDto);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Review from the owner is not yet available.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Pomoćne metode

        public void LoadReservations()
        {
            var reservationsList = _displayService.GetReservationsForGuest(Session.CurrentUser.Id);
            Reservations = new ObservableCollection<ReservationDetailsDTO>(reservationsList);
            OnPropertyChanged(nameof(Reservations));
        }

        private bool IsReportDataValid(out (List<ReservationDetailsDTO> FilteredReservations, DateTime StartDate, DateTime EndDate) data)
        {
            data = default;
            if (!ReportStartDate.HasValue || !ReportEndDate.HasValue) { MessageBox.Show("Please select both a start and an end date."); return false; }
            if (ReportStartDate.Value > ReportEndDate.Value) { MessageBox.Show("The start date cannot be after the end date."); return false; }

            var filtered = Reservations.Where(r => r.StartDate >= ReportStartDate.Value && r.StartDate <= ReportEndDate.Value).ToList();
            if (!filtered.Any()) { MessageBox.Show("There are no reservations in the selected period."); return false; }

            data = (filtered, ReportStartDate.Value, ReportEndDate.Value);
            return true;
        }

        #endregion
    }
}