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
        private List<Reservation> _myReservations;

        private readonly AccommodationRepository _accommodationRepository;
        private List<Accommodation> _myAccommodations; //dobra zamisao ali ne treba ucitavati sve smjestaje, vec samo ove koje imaju Id guEST
        public MyReservationsView()
        {
            InitializeComponent();
            _reservationRepository = new ReservationRepository();
            _accommodationRepository = new AccommodationRepository();

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
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
