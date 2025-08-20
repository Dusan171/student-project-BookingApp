using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Services;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.View.Guest
{
    /// <summary>
    /// Interaction logic for AccommodationReservationView.xaml
    /// </summary>
    public partial class AccommodationReservationView : Window
    {
        private readonly Accommodation _accommodation;
        private readonly IOccupiedDateRepository _occupiedDateRepository;
        private readonly IReservationService _reservationService;
        public AccommodationReservationView(Accommodation accommodation)
        {
            InitializeComponent();
            _accommodation = accommodation;

            var reservationRepository = new ReservationRepository();

            _occupiedDateRepository = Injector.CreateInstance<IOccupiedDateRepository>();
            _reservationService = Injector.CreateInstance<IReservationService>();

            LoadAndDisplayOccupiedDates();
        }
        private void LoadAndDisplayOccupiedDates()
        {
            var occupiedDates = _occupiedDateRepository.GetByAccommodationId(_accommodation.Id);
            foreach (var date in occupiedDates)
            {
                //kreiranje jednog po jednog blackout datuma
                CalendarDateRange range = new CalendarDateRange(date.Date);
                StartDatePicker.BlackoutDates.Add(range);
                EndDatePicker.BlackoutDates.Add(range);
            }
        }
       
        private void Reserve_Click(object sender, RoutedEventArgs e)
        {
            // 1. Prikupljanje i osnovna validacija unosa sa korisničkog interfejsa.
            // Ovaj deo logike pripada View-u jer direktno radi sa kontrolama (DatePicker, TextBox).

            if (!StartDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a start date.");
                return;
            }

            if (!EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select an end date.");
                return;
            }

            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            if (endDate <= startDate)
            {
                MessageBox.Show("End date must be after the start date.");
                return;
            }

            if (!int.TryParse(GuestsTextBox.Text, out int guestNumber) || guestNumber <= 0)
            {
                MessageBox.Show("Please enter a valid number of guests.");
                return;
            }

            // 2. Poziv servisnog sloja da izvrši poslovnu logiku.
            // Sav "pametan" posao je sada sakriven iza jedne metode.
            try
            {
                //ovdje se poziva _reservationService koji dobije od Injectora
                _reservationService.Create(_accommodation, startDate, endDate, guestNumber);

                MessageBox.Show("Reservation successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // Dobra praksa: signalizira da je prozor uspešno završio operaciju
                this.Close();
            }
            catch (Exception ex)
            {
                // Prikazujemo greške koje dolaze iz poslovne logike (npr. "termin je zauzet").
                MessageBox.Show(ex.Message, "Reservation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
