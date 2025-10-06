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
        private readonly IReservationService _reservationService;
        private readonly IReservationDisplayService _reservationDisplayService;
        private readonly IAccommodationReviewService _accommodationReviewService;
        private readonly IGuestReviewService _guestReviewService;
        private readonly IReservationCancellationService _cancellationService;

        public static event Action NavigateBackToSearchRequested;

        #region Svojstva
        public ObservableCollection<ReservationDetailsDTO> Reservations { get; set; }

        private DateTime? _reportStartDate;
        public DateTime? ReportStartDate
        {
            get => _reportStartDate;
            set { _reportStartDate = value; OnPropertyChanged(); }
        }

        private DateTime? _reportEndDate;
        public DateTime? ReportEndDate
        {
            get => _reportEndDate;
            set { _reportEndDate = value; OnPropertyChanged(); }
        }

        #endregion

        #region Komande
        public ICommand RescheduleCommand { get; }
        public ICommand RateCommand { get; }
        public ICommand ViewReviewCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand CancelReservationCommand { get; }
        public ICommand GenerateReportCommand { get; }
        #endregion

        public MyReservationsViewModel()
        {
            _reservationService = Injector.CreateInstance<IReservationService>();
            _reservationDisplayService = Injector.CreateInstance<IReservationDisplayService>();
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();
            _guestReviewService = Injector.CreateInstance<IGuestReviewService>();
            _cancellationService = Injector.CreateInstance<IReservationCancellationService>();

            RescheduleCommand = new RelayCommand(Reschedule);
            RateCommand = new RelayCommand(Rate);
            ViewReviewCommand = new RelayCommand(ViewReview);
            CloseCommand = new RelayCommand(GoBackToSearch);
            CancelReservationCommand = new RelayCommand(CancelReservation);
            GenerateReportCommand = new RelayCommand(GenerateReport);

            LoadReservations();
        }

        #region Logika Komandi

        private void GenerateReport(object parameter)
        {
            if (!ReportStartDate.HasValue || !ReportEndDate.HasValue)
            {
                MessageBox.Show("Please select both a start and an end date for the report.", "Date Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ReportStartDate.Value > ReportEndDate.Value)
            {
                MessageBox.Show("The start date cannot be after the end date.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<ReservationDetailsDTO> filteredReservations = Reservations
                .Where(r => r.StartDate >= ReportStartDate.Value && r.StartDate <= ReportEndDate.Value)
                .ToList();

            if (!filteredReservations.Any())
            {
                MessageBox.Show("There are no reservations in the selected period.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var pdfService = new PdfReportService();
                pdfService.GenerateReservationReport(filteredReservations, ReportStartDate.Value, ReportEndDate.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while generating the PDF report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelReservation(object parameter)
        {
            if (parameter is not ReservationDetailsDTO selectedReservation) return;

            var result = MessageBox.Show(
                $"Are you sure you want to cancel your reservation for:\n\n{selectedReservation.AccommodationName}\n({selectedReservation.StartDate:dd.MM.yyyy} - {selectedReservation.EndDate:dd.MM.yyyy})?",
                "Confirm Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _cancellationService.CancelReservation(selectedReservation.ReservationId);
                    MessageBox.Show("Reservation successfully cancelled.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadReservations();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cancellation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Reschedule(object parameter)
        {
            var selectedDto = parameter as ReservationDetailsDTO;
            if (selectedDto == null) return;

            var rescheduleWindow = new RescheduleRequestView(selectedDto);
            ShowDialogAndRefresh(rescheduleWindow);
        }

        private void Rate(object parameter)
        {
            var selectedDto = parameter as ReservationDetailsDTO;
            if (selectedDto == null) return;

            var rateWindow = new AccommodationReviewView(selectedDto);
            ShowDialogAndRefresh(rateWindow);
        }

        private void ViewReview(object parameter)
        {
            var selectedDto = parameter as ReservationDetailsDTO;
            if (selectedDto == null) return;

            GuestReviewDTO reviewDto = _guestReviewService.GetReviewForReservation(selectedDto.ReservationId);

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

        private void GoBackToSearch(object obj)
        {
            NavigateBackToSearchRequested?.Invoke();
        }

        #endregion

        #region Pomoćne metode

        public void LoadReservations()
        {
            var reservationsList = _reservationDisplayService.GetReservationsForGuest(Session.CurrentUser.Id);
            Reservations = new ObservableCollection<ReservationDetailsDTO>(reservationsList);
            OnPropertyChanged(nameof(Reservations));
        }

        private void ShowDialogAndRefresh(Window dialogWindow)
        {
            dialogWindow.ShowDialog();
            LoadReservations();
        }
        #endregion
    }
}