using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Guest;
using BookingApp.Presentation.ViewModel.Guest;
//using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel
{
    public class MyReservationsViewModel : ViewModelBase
    {
        private readonly IReservationDisplayService _reservationDisplayService;
        private readonly IReviewService _reviewService;

        #region Svojstva
        public ObservableCollection<MyReservationViewModel> Reservations { get; set; }

        private MyReservationViewModel _selectedReservation;
        public MyReservationViewModel SelectedReservation
        {
            get => _selectedReservation;
            set { _selectedReservation = value; OnPropertyChanged(); }
        }
        #endregion

        #region Komande
        public ICommand RescheduleCommand { get; }
        public ICommand RateCommand { get; }
        public ICommand ViewReviewCommand { get; }
        public ICommand CloseCommand { get; }
        #endregion

        public MyReservationsViewModel()
        {
            _reservationDisplayService = Injector.CreateInstance<IReservationDisplayService>();
            _reviewService = Injector.CreateInstance<IReviewService>();

            // --- ISPRAVNA INICIJALIZACIJA ZA OVAJ XAML ---
            // SVE akcije zavise od selektovanog reda
            RescheduleCommand = new RelayCommand(Reschedule, CanReschedule);
            RateCommand = new RelayCommand(Rate, CanExecuteOnSelectedItem);
            ViewReviewCommand = new RelayCommand(ViewReview, CanExecuteOnSelectedItem);
            CloseCommand = new RelayCommand(Close);

            LoadReservations();
        }

        #region Logika
        public void LoadReservations()
        {
            var reservationsList = _reservationDisplayService.GetReservationsForGuest(Session.CurrentUser.Id);
            Reservations = new ObservableCollection<MyReservationViewModel>(reservationsList);
            OnPropertyChanged(nameof(Reservations));
        }

        private bool CanExecuteOnSelectedItem(object obj)
        {
            return SelectedReservation != null;
        }

        // --- NOVA CanReschedule METODA ---
        private bool CanReschedule(object obj)
        {
            // Dugme je omogućeno samo ako je red selektovan I ako je IsRescheduleEnabled za taj red true
            return SelectedReservation != null && SelectedReservation.IsRescheduleEnabled;
        }

        // --- ISPRAVLJENA RESCHEDULE METODA ---
        private void Reschedule(object obj)
        {
            // Metoda sada radi sa `SelectedReservation` svojstvom, kao i ostale
            if (SelectedReservation == null) return;

            var rescheduleWindow = new RescheduleRequestView(SelectedReservation.OriginalReservation);
            rescheduleWindow.ShowDialog();
            LoadReservations();
        }

        private void Rate(object obj)
        {
            if (SelectedReservation == null) return;
            Reservation selected = SelectedReservation.OriginalReservation;

            // ... (ostatak Rate logike ostaje isti)
        }

        private void ViewReview(object obj)
        {
            if (SelectedReservation == null) return;
            Reservation selected = SelectedReservation.OriginalReservation;

            // ... (ostatak ViewReview logike ostaje isti)
        }

        private void Close(object obj)
        {
            Application.Current.Windows.OfType<MyReservationsView>().FirstOrDefault()?.Close();
        }
        #endregion
    }
}