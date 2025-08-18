using System;
using System.Linq;
using System.Windows;
using BookingApp.Utilities;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Controls;
using BookingApp.Domain;
using BookingApp.Repositories;

namespace BookingApp.Presentation.Guest
{
    /// <summary>
    /// Interaction logic for AccommodationReservationView.xaml
    /// </summary>
    public partial class AccommodationReservationView : Window
    {
        private readonly Accommodation _accommodation;
        private readonly ReservationRepository _reservationRepository;
        private readonly OccupiedDateRepository _occupiedDateRepository;
        public AccommodationReservationView(Accommodation accommodation)
        {
            InitializeComponent();
            _accommodation = accommodation;
            _reservationRepository = new ReservationRepository();
            _occupiedDateRepository = new OccupiedDateRepository();
            HighlightOccupiedDates();
        }
        private void HighlightOccupiedDates()
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

            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.");
                return;
            }
            

            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            if (endDate <= startDate)
            {
                MessageBox.Show("End date must be after start date.");
                return;
            }

            if (!int.TryParse(GuestsTextBox.Text, out int guestNumber))
            {
                MessageBox.Show("Please enter a valid number of guests.");
                return;
            }

            //pozivanje metode iz repozitory
            try
            {
                _reservationRepository.CreateReservation(_accommodation, startDate, endDate,guestNumber,_occupiedDateRepository);

                MessageBox.Show("Reservation successful.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);   
            }
        }
    }
}
