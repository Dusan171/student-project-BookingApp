using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BookingApp.Utilities;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Presentation.ViewModels;

namespace BookingApp.Presentation.Guest
{
    /// <summary>
    /// Interaction logic for MyReservationsView.xaml
    /// </summary>
    public partial class MyReservationsView : Window
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly AccommodationRepository _accommodationRepository;
        private readonly RescheduleRequestRepository _rescheduleRequestRepository;
        private readonly GuestReviewRepository _guestReviewRepository;
        private readonly OwnerReviewRepository _ownerReviewRepository;

       // private List<Reservation> _myReservations;
        //private List<Accommodation> _myAccommodations; //dobra zamisao ali ne treba ucitavati sve smjestaje, vec samo ove koje imaju Id guEST
        public List<MyReservationViewModel> MyReservationsDisplay { get; set; }

        public MyReservationsView()
        {
            InitializeComponent();
            DataContext = this;

            _reservationRepository = new ReservationRepository();
            _accommodationRepository = new AccommodationRepository();
            _rescheduleRequestRepository = new RescheduleRequestRepository();

            _guestReviewRepository = new GuestReviewRepository();
            _ownerReviewRepository = new OwnerReviewRepository();

            LoadReservations();

        }

        private void LoadReservations()
        {
            var allAccommodations = _accommodationRepository.GetAll();
            var myReservations = _reservationRepository.GetByGuestId(Session.CurrentUser.Id).OrderByDescending(r => r.StartDate).ToList();

            MyReservationsDisplay = new List<MyReservationViewModel>();

            foreach (var reservation in myReservations)
            {
                var accommodation = allAccommodations.FirstOrDefault(a => a.Id == reservation.AccommodationId);
                var request = _rescheduleRequestRepository.GetByReservationId(reservation.Id);

                var viewModel = new MyReservationViewModel
                {
                    ReservationId = reservation.Id,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    GuestsNumber = reservation.GuestsNumber,
                    AccommodationName = accommodation?.Name ?? "N/A",
                    RequestStatusText = request?.Status.ToString() ?? "Not requested",
                    OwnerComment = request?.OwnerComment ?? "",
                    OriginalReservation = reservation
                };
                bool hasPendingRequest = (request != null && request.Status == RequestStatus.Pending);
                viewModel.IsRescheduleEnabled = reservation.StartDate > DateTime.Now && !hasPendingRequest;

                MyReservationsDisplay.Add(viewModel);
            }
            ReservationsDataGrid.ItemsSource = MyReservationsDisplay;
            //ucitavanje rezervacija samo trenutno logovanog korisnika
            //_myReservations = _reservationRepository.GetByGuestId(Session.CurrentUser.Id).OrderByDescending(r => r.StartDate).ToList();

            //ReservationsDataGrid.ItemsSource = _myReservations;
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

            //otvaranje prozora za ocjenjivanje
            var reviewWind = new GuestReviewView(selectedReservation);
            reviewWind.Show();
        }
        private void ViewReviewButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedViewModel = ReservationsDataGrid.SelectedItem as MyReservationViewModel;
            if (selectedViewModel == null)
            {
                MessageBox.Show("Izaberite prvo smjestaj.", "Obavestenje", MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            // USLOV: Proveravamo da li je gost već ocenio vlasnika za ovu rezervaciju
            // VAŽNO: Morate imati metodu kao što je "HasGuestRated" u vašem AccommodationReviewRepository
            Reservation selectedReservation = selectedViewModel.OriginalReservation;

            bool guestHasRated = _ownerReviewRepository.HasGuestRated(selectedReservation.Id);

            if (!guestHasRated)
            {
                MessageBox.Show("Morate prvo vi oceniti boravak da biste mogli da vidite recenziju vlasnika.", "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ako je uslov ispunjen, tražimo recenziju od vlasnika
            //treba mi aleksandrino a ne moje
            GuestReview reviewFromOwner = _guestReviewRepository.GetByReservationId(selectedReservation).FirstOrDefault();

            if (reviewFromOwner == null)
            {
                MessageBox.Show("Vlasnik još uvek nije ostavio recenziju za vaš boravak.", "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Ako sve prođe, otvaramo prozor za prikaz recenzije
            OwnerReviewDetailsView detailsWindow = new OwnerReviewDetailsView(reviewFromOwner);
            detailsWindow.ShowDialog();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
