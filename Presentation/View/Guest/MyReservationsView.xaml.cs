using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BookingApp.Utilities;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.View.Guest
{
    /// <summary>
    /// Interaction logic for MyReservationsView.xaml
    /// </summary>
    public partial class MyReservationsView : Window
    {
        //private readonly IGuestReviewRepository _guestReviewRepository;
        //private readonly IOwnerReviewRepository _ownerReviewRepository;

        private readonly IReviewService _reviewService;

        private readonly IReservationDisplayService _reservationDisplayService;

       // private List<Reservation> _myReservations;
        //private List<Accommodation> _myAccommodations; //dobra zamisao ali ne treba ucitavati sve smjestaje, vec samo ove koje imaju Id guEST
        public List<MyReservationViewModel> MyReservationsDisplay { get; set; }

        public MyReservationsView()
        {
            InitializeComponent();
            DataContext = this;

            /* var reservationRepository = new ReservationRepository();
             var accommodationRepository = new AccommodationRepository();
             var rescheduleRequestRepository = new RescheduleRequestRepository();
             var ownerReviewRepository = new OwnerReviewRepository();

             _reviewService = new ReviewService(ownerReviewRepository);
             _guestReviewRepository = Injector.CreateInstance<GuestReviewRepository>();
             _ownerReviewRepository = Injector.CreateInstance<OwnerReviewRepository>();

             _reservationDisplayService = new ReservationDisplayService(reservationRepository, accommodationRepository, rescheduleRequestRepository);*/
            _reviewService = Injector.CreateInstance<IReviewService>();
            _reservationDisplayService = Injector.CreateInstance<IReservationDisplayService>();
            LoadReservations();

        }

        private void LoadReservations()
        {
            MyReservationsDisplay = _reservationDisplayService.GetReservationsForGuest(Session.CurrentUser.Id);
            ReservationsDataGrid.ItemsSource = MyReservationsDisplay;
        }
        private void RescheduleButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedViewModel = ReservationsDataGrid.SelectedItem as MyReservationViewModel;
            if (selectedViewModel == null) return;

            var rescheduleWindow = new RescheduleRequestView(selectedViewModel.OriginalReservation);
            rescheduleWindow.ShowDialog();

            LoadReservations();
        }
        private void RateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedViewModel = ReservationsDataGrid.SelectedItem as MyReservationViewModel;
            if (selectedViewModel == null)
            {
                MessageBox.Show("Please select a reservation to rate.");
                return;
            }

            Reservation selectedReservation = selectedViewModel.OriginalReservation;
            //Provjera da li je boravak zavrsen
            if (DateTime.Now < selectedReservation.EndDate)
            {
                MessageBox.Show("You can only rate after your stay has ended.");
                return;
            }

            if (_reviewService.IsReviewPeriodExpired(selectedReservation))
            {
                MessageBox.Show("The period for leaving a review has expired (5 days).", "Review Period Expired", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Prekidamo izvršavanje, prozor se neće ni otvoriti
            }

            //otvaranje prozora za ocjenjivanje
            var reviewWind = new GuestReviewView(selectedReservation);
            reviewWind.ShowDialog();
        }
        private void ViewReviewButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedViewModel = ReservationsDataGrid.SelectedItem as MyReservationViewModel;
            if (selectedViewModel == null)
            {
                MessageBox.Show("Please select a reservation.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Reservation selectedReservation = selectedViewModel.OriginalReservation;

            try
            {
                // --- ISPRAVKA: Sva logika je sada u servisu ---
                if (!_reviewService.HasGuestRated(selectedReservation.Id))
                {
                    MessageBox.Show("You must first rate your stay to see the owner's review.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                GuestReview reviewFromOwner = _reviewService.GetReviewFromOwner(selectedReservation);
                if (reviewFromOwner == null)
                {
                    MessageBox.Show("The owner has not yet left a review for your stay.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                OwnerReviewDetailsView detailsWindow = new OwnerReviewDetailsView(reviewFromOwner);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}