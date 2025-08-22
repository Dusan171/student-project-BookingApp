using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Guest;
using BookingApp.Services;
using BookingApp.Services.DTOs;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel
{
    public class MyReservationsViewModel : ViewModelBase
    {
        private readonly IReservationDisplayService _reservationDisplayService;
        private readonly IAccommodationReviewService _accommodationReviewService;
        private readonly IGuestReviewService _guestReviewService;
        

        #region Svojstva
        public ObservableCollection<ReservationDetailsDTO> Reservations { get; set; }

        private ReservationDetailsDTO _selectedReservation;
        public ReservationDetailsDTO SelectedReservation
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
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();
            _guestReviewService = Injector.CreateInstance<IGuestReviewService>();

            // --- ISPRAVNA INICIJALIZACIJA ZA OVAJ XAML ---
            // SVE akcije zavise od selektovanog reda
            RescheduleCommand = new RelayCommand(Reschedule, CanReschedule);
            RateCommand = new RelayCommand(Rate, CanRate);
            ViewReviewCommand = new RelayCommand(ViewReview, CanViewReview);
            CloseCommand = new RelayCommand(Close);

            LoadReservations();
        }

        #region Logika
        public void LoadReservations()
        {
            var reservationsList = _reservationDisplayService.GetReservationsForGuest(Session.CurrentUser.Id);
            Reservations = new ObservableCollection<ReservationDetailsDTO>(reservationsList);
            OnPropertyChanged(nameof(Reservations));
        }
        // --- NOVA CanReschedule METODA ---
        private bool CanReschedule(object obj)
        {
            // Dugme je omogućeno samo ako je red selektovan I ako je IsRescheduleEnabled za taj red true
            return SelectedReservation != null && SelectedReservation.IsRescheduleEnabled;
        }
        // --- PROMENA #4: Nova, specifična CanExecute metoda za "Rate" ---
        private bool CanRate(object obj)
        {
            // Logika je sada trivijalna jer se oslanja na pripremljen DTO
            return SelectedReservation != null && SelectedReservation.IsRatingEnabled;
        }

        // --- PROMENA #5: Nova, specifična CanExecute metoda za "View Review" ---
        private bool CanViewReview(object obj)
        {
            return SelectedReservation != null && SelectedReservation.IsGuestReviewVisible;
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

            // Kreiramo View za ocenjivanje i prosleđujemo mu originalnu rezervaciju
            var rateWindow = new GuestReviewView(SelectedReservation.OriginalReservation);
            rateWindow.ShowDialog();

            // Nakon što se prozor za ocenjivanje zatvori, osvežavamo listu
            // Ovo će ažurirati DTO i onemogućiti "Rate" i omogućiti "View Review" (ako su uslovi ispunjeni)
            LoadReservations();
        }

        private void ViewReview(object obj)
        {
            // 1. Provera da li je nešto selektovano
            if (SelectedReservation == null) return;

            // 2. Dobavljanje recenzije od vlasnika pomoću servisa
            GuestReview reviewFromOwner = _guestReviewService.GetReviewForReservation(SelectedReservation.OriginalReservation.Id);

            // 3. Provera da li recenzija postoji
            if (reviewFromOwner != null)
            {
                // Ako postoji, prikazujemo njene detalje koristeći MessageBox.Show
                MessageBox.Show(
                    $"Rating for cleanliness: {reviewFromOwner.CleanlinessRating}\n" +
                    $"Rating for rule following: {reviewFromOwner.RuleRespectingRating}\n" +
                    $"Comment: {reviewFromOwner.Comment}",
                    "Review from Owner",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                // Ako ne postoji, prikazujemo poruku o grešci
                MessageBox.Show(
                    "Review from the owner could not be found.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Close(object obj)
        {
            Application.Current.Windows.OfType<MyReservationsView>().FirstOrDefault()?.Close();
        }
        #endregion
    }
}