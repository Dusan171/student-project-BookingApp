using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Utilities;
using BookingApp.Services;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.View.Guest
{
    /// <summary>
    /// Interaction logic for RescheduleRequestView.xaml
    /// </summary>
    public partial class RescheduleRequestView : Window
    {
        private readonly Reservation _reservation;
        private readonly Accommodation _accommodation;
        private readonly IRescheduleRequestService _rescheduleRequestService;
        private readonly IAccommodationRepository _accommodationRepository;
        //private readonly OccupiedDateRepository _occupiedDateRepository;
        //private readonly RescheduleRequestRepository _rescheduleRequestRepository;
        //private readonly AccommodationRepository _accommodationRepository;

        public RescheduleRequestView(Reservation reservatiion)
        {
            InitializeComponent();
            _reservation = reservatiion;

            /*var occupiedDateRepository = new OccupiedDateRepository();
            var rescheduleRequestRepository = new RescheduleRequestRepository();
            var accommodationRepository = new AccommodationRepository();
            _rescheduleRequestService = new RescheduleRequestService(occupiedDateRepository, rescheduleRequestRepository, accommodationRepository);

            //ucitavanje smjestaja na osnovu Id-a rezervacije

            _accommodation = accommodationRepository.GetAll().FirstOrDefault(a => a.Id == _reservation.AccommodationId);

            if (_accommodation == null)
            {
                MessageBox.Show("Could not find accommodation details. The window will close.");
                this.Close();
                return;
            }*/
            // --- ISPRAVKA: Dobijamo SVE zavisnosti od Injector-a ---
            _rescheduleRequestService = Injector.CreateInstance<IRescheduleRequestService>();
            _accommodationRepository = Injector.CreateInstance<IAccommodationRepository>();

            // Dohvatanje smeštaja se sada radi preko interfejsa koji smo dobili
            _accommodation = _accommodationRepository.GetAll().FirstOrDefault(a => a.Id == _reservation.AccommodationId);

            // Provera da li je smeštaj pronađen
            if (_accommodation == null)
            {
                MessageBox.Show("Could not find accommodation details. The window will close.");
                this.Close();
                return; // Važno je prekinuti izvršavanje konstruktora
            }
            LoadInitialData();
        }
        private void LoadInitialData()
        {
            // Prikaz osnovnih informacija
            AccommodationNameTextBlock.Text = _accommodation.Name;
            CurrentPeriodTextBlock.Text = $"{_reservation.StartDate:dd.MM.yyyy} - {_reservation.EndDate:dd.MM.yyyy}";

            // Prikaz zauzetih datuma (logika je sada u servisu)
            var blackoutDates = _rescheduleRequestService.GetBlackoutDatesForReschedule(_reservation);
            foreach (var date in blackoutDates)
            {
                var range = new CalendarDateRange(date);
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

            try
            {
                // 2. Pozivamo servis da obavi sav posao
                _rescheduleRequestService.CreateRequest(_reservation, newStartDate, newEndDate);

                MessageBox.Show("Your request has been sent to the owner.", "Request Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
