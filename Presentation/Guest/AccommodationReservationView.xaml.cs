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

            if (guestNumber > _accommodation.MaxGuests)
            {
                MessageBox.Show($"Max allowed guests: {_accommodation.MaxGuests}");
                return;
            }

            int stayLength = (endDate - startDate).Days;
            if (stayLength < _accommodation.MinReservationDays)
            {
                MessageBox.Show($"Minimum stay is {_accommodation.MinReservationDays} days.");
                return;
            }

            // Check for overlap with occupied dates
            var occupiedDates = _occupiedDateRepository.GetByAccommodationId(_accommodation.Id);
            bool overlap = Enumerable.Range(0, stayLength)
                .Select(offset => startDate.AddDays(offset).Date)
                .Any(date => occupiedDates.Any(o => o.Date == date));

            if (overlap)
            {
                MessageBox.Show("Selected period is not available.");
                return;
            }

            // Save reservation
            var reservation = new Reservation
            {
                Id = _reservationRepository.NextId(),
                AccommodationId = _accommodation.Id,
                GuestId = Session.CurrentUser.Id,
                StartDate = startDate,
                EndDate = endDate,
                GuestsNumber = guestNumber,
                Status = ReservationStatus.Active
            };

            _reservationRepository.Save(reservation);

            // Save occupied dates
            List<OccupiedDate> occupiedDatesToSave = new List<OccupiedDate>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                occupiedDatesToSave.Add(new OccupiedDate
                {
                    Id = _occupiedDateRepository.NextId(),
                    AccommodationId = _accommodation.Id,
                    ReservationId = reservation.Id,
                    Date = date
                });
            }
            _occupiedDateRepository.Save(occupiedDatesToSave);

            MessageBox.Show("Reservation successful.");
            this.Close();
        }
    }
}
