using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BookingApp.Utilities;
using BookingApp.Domain;
using BookingApp.Repositories;

namespace BookingApp.Presentation.Guest
{
    /// <summary>
    /// Interaction logic for MyReservationsView.xaml
    /// </summary>
    public partial class MyReservationsView : Window
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly AccommodationRepository _accommodationRepository;
        private readonly GuestReviewRepository _guestReviewRepository;
        private readonly OwnerReviewRepository _ownerReviewRepository;

        private List<Reservation> _myReservations;
        private List<Accommodation> _myAccommodations; //dobra zamisao ali ne treba ucitavati sve smjestaje, vec samo ove koje imaju Id guEST

        public MyReservationsView()
        {
            InitializeComponent();
            _reservationRepository = new ReservationRepository();
            _accommodationRepository = new AccommodationRepository();

            _guestReviewRepository = new GuestReviewRepository();
            _ownerReviewRepository = new OwnerReviewRepository();

            LoadReservations();

        }

        private void LoadReservations()
        {
            //ucitavanje rezervacija samo trenutno logovanog korisnika
            _myReservations = _reservationRepository.GetByGuestId(Session.CurrentUser.Id).OrderByDescending(r => r.StartDate).ToList();

            ReservationsDataGrid.ItemsSource = _myReservations;
        }
        private void RateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedReservation = (Reservation)ReservationsDataGrid.SelectedItem;
            if (selectedReservation == null)
            {
                MessageBox.Show("Please select a reservation to rate.");
                return;
            }
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
            var selectedReservation = (Reservation)ReservationsDataGrid.SelectedItem;
            if (selectedReservation == null)
            {
                MessageBox.Show("Izaberite prvo smjestaj.", "Obavestenje", MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            // USLOV: Proveravamo da li je gost već ocenio vlasnika za ovu rezervaciju
            // VAŽNO: Morate imati metodu kao što je "HasGuestRated" u vašem AccommodationReviewRepository
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
