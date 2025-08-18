using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Utilities;

namespace BookingApp.Presentation.Guest
{
    /// <summary>
    /// Interaction logic for RescheduleRequestView.xaml
    /// </summary>
    public partial class RescheduleRequestView : Window
    {
        private readonly Reservation _reservation;
        private readonly Accommodation _accommodation;
        private readonly OccupiedDateRepository _occupiedDateRepository;
        private readonly RescheduleRequestRepository _rescheduleRequestRepository;
        private readonly AccommodationRepository _accommodationRepository;

        public RescheduleRequestView(Reservation reservatiion)
        {
            InitializeComponent();
            _reservation = reservatiion;

            _occupiedDateRepository = new OccupiedDateRepository();
            _rescheduleRequestRepository = new RescheduleRequestRepository();
            _accommodationRepository = new AccommodationRepository();

            //ucitavanje smjestaja na osnovu Id-a rezervacije

            _accommodation = _accommodationRepository.GetAll().FirstOrDefault(a => a.Id == _reservation.AccommodationId);

            if (_accommodation == null)
            {
                MessageBox.Show("Could not find accommodation details. The window will close.");
                this.Close();
                return;
            }
            InitializeData();
            HighlightOccupiedDates();
        }
        private void InitializeData() 
        {
            AccommodationNameTextBlock.Text = _accommodation.Name;
            CurrentPeriodTextBlock.Text = $"{_reservation.StartDate:dd.MM.yyyy} - {_reservation.EndDate:dd.MM.yyyy}";
        }
        private void HighlightOccupiedDates()
        {
            // 1. Uzimamo SVE zauzete datume za ovaj smeštaj
            var allOccupiedDates = _occupiedDateRepository.GetByAccommodationId(_accommodation.Id);

            // 2. Kreiramo listu datuma koji pripadaju TRENUTNOJ rezervaciji
            var currentReservationDates = Enumerable.Range(0, (_reservation.EndDate - _reservation.StartDate).Days)
                                                    .Select(offset => _reservation.StartDate.AddDays(offset).Date)
                                                    .ToList();

            // 3. Filtriramo zauzete datume - izbacujemo one koji pripadaju našoj rezervaciji
            var otherOccupiedDates = allOccupiedDates.Where(od => !currentReservationDates.Contains(od.Date.Date)).ToList();

            // 4. Onemogućavamo samo datume drugih rezervacija
            foreach (var date in otherOccupiedDates)
            {
                CalendarDateRange range = new CalendarDateRange(date.Date);
                NewStartDatePicker.BlackoutDates.Add(range);
                NewEndDatePicker.BlackoutDates.Add(range);
            }
        }
        private void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            // --- Validacija unosa ---
            if (!NewStartDatePicker.SelectedDate.HasValue || !NewEndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both new start and end dates.");
                return;
            }

            DateTime newStartDate = NewStartDatePicker.SelectedDate.Value.Date;
            DateTime newEndDate = NewEndDatePicker.SelectedDate.Value.Date;

            if (newEndDate <= newStartDate)
            {
                MessageBox.Show("End date must be after start date.");
                return;
            }

            int stayLength = (newEndDate - newStartDate).Days;
            if (stayLength < _accommodation.MinReservationDays)
            {
                MessageBox.Show($"Minimum stay is {_accommodation.MinReservationDays} days.");
                return;
            }

            // --- Provera preklapanja sa zauzetim terminima (kao kod kreiranja rezervacije) ---
            var occupiedDates = NewStartDatePicker.BlackoutDates.Select(range => range.Start).ToList();
            bool isOverlap = Enumerable.Range(0, stayLength)
                .Select(offset => newStartDate.AddDays(offset))
                .Any(date => occupiedDates.Contains(date));

            if (isOverlap)
            {
                MessageBox.Show("Selected period overlaps with another reservation and is not available.");
                return;
            }

            // --- Kreiranje i čuvanje zahteva ---
            var newRequest = new RescheduleRequest
            {
                ReservationId = _reservation.Id,
                GuestId = Session.CurrentUser.Id,
                NewStartDate = newStartDate,
                NewEndDate = newEndDate,
                Status = RequestStatus.Pending,
                IsSeenByGuest = false // Nije relevantno dok vlasnik ne odgovori
            };

            _rescheduleRequestRepository.Save(newRequest);

            MessageBox.Show("Your request has been sent to the owner.", "Request Sent", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
