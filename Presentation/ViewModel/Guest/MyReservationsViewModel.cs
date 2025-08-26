using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text; // Potrebno za StringBuilder
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.View.Guest;
using BookingApp.Services;
using BookingApp.Services.DTO;
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
            // --- ZAVISNOSTI I KOMANDE (BEZ PROMENA) ---
            _reservationDisplayService = Injector.CreateInstance<IReservationDisplayService>();
            _accommodationReviewService = Injector.CreateInstance<IAccommodationReviewService>();
            _guestReviewService = Injector.CreateInstance<IGuestReviewService>();

            RescheduleCommand = new RelayCommand(Reschedule, CanReschedule);
            RateCommand = new RelayCommand(Rate, CanRate);
            ViewReviewCommand = new RelayCommand(ViewReview, CanViewReview);
            CloseCommand = new RelayCommand(Close);

            // Inicijalno učitavanje podataka
            LoadReservations();
        }

        #region Logika Komandi

        private void Reschedule(object obj)
        {
            if (SelectedReservation == null) return;

            // --- PROMENA: Koristimo pomoćnu metodu ---
            var rescheduleWindow = new RescheduleRequestView(SelectedReservation.OriginalReservation);
            ShowDialogAndRefresh(rescheduleWindow);
        }

        private void Rate(object obj)
        {
            if (SelectedReservation == null) return;

            // --- PROMENA: Koristimo pomoćnu metodu ---
            var rateWindow = new AccommodationReviewView(SelectedReservation.OriginalReservation);
            ShowDialogAndRefresh(rateWindow);
        }

        private void ViewReview(object obj)
        {
            if (SelectedReservation == null) return;

            GuestReviewDTO reviewFromOwner = _guestReviewService.GetReviewForReservation(SelectedReservation.OriginalReservation.Id);

            if (reviewFromOwner != null)
            {
                // --- PROMENA: Logika za kreiranje poruke je sada u pomoćnoj metodi ---
                string message = FormatReviewMessage(reviewFromOwner.ToGuestReview());
                MessageBox.Show(message, "Review from Owner", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Review from the owner could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close(object obj)
        {
            Application.Current.Windows.OfType<MyReservationsView>().FirstOrDefault()?.Close();
        }

        #endregion

        #region Pomoćne metode

        // --- CanExecute metode (BEZ PROMENA) ---
        private bool CanReschedule(object obj)
        {
            return SelectedReservation != null && SelectedReservation.IsRescheduleEnabled;
        }

        private bool CanRate(object obj)
        {
            return SelectedReservation != null && SelectedReservation.IsRatingEnabled;
        }

        private bool CanViewReview(object obj)
        {
            return SelectedReservation != null && SelectedReservation.IsGuestReviewVisible;
        }

        // --- Metoda za učitavanje (BEZ PROMENA) ---
        public void LoadReservations()
        {
            var reservationsList = _reservationDisplayService.GetReservationsForGuest(Session.CurrentUser.Id);
            Reservations = new ObservableCollection<ReservationDetailsDTO>(reservationsList);
            OnPropertyChanged(nameof(Reservations));
        }

        // --- NOVO: Pomoćna metoda za prikazivanje prozora i osvežavanje (DRY princip) ---
        private void ShowDialogAndRefresh(Window dialogWindow)
        {
            dialogWindow.ShowDialog();
            LoadReservations();
        }

        // --- NOVO: Pomoćna metoda koja samo formatira string poruke ---
        private string FormatReviewMessage(GuestReview review)
        {
            // Korišćenje StringBuilder-a je dobra praksa za spajanje više delova teksta
            var sb = new StringBuilder();
            sb.AppendLine($"Rating for cleanliness: {review.CleanlinessRating}");
            sb.AppendLine($"Rating for rule following: {review.RuleRespectingRating}");
            sb.AppendLine($"Comment: {review.Comment}");
            return sb.ToString();
        }

        #endregion
    }
}