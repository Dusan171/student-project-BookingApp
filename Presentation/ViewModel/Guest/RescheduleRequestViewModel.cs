using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel
{
    public class RescheduleRequestViewModel : ViewModelBase
    {
        private readonly Reservation _reservation;
        private readonly Accommodation _accommodation;
        private readonly IRescheduleRequestService _rescheduleRequestService;
        private readonly IAccommodationRepository _accommodationRepository;

        // Akcija za zatvaranje prozora
        public Action CloseAction { get; set; }

        #region Svojstva za povezivanje (Binding)

        // Svojstva samo za čitanje, za prikaz informacija
        public string AccommodationName => _accommodation?.Name;
        public string CurrentPeriod => $"{_reservation.StartDate:dd.MM.yyyy} - {_reservation.EndDate:dd.MM.yyyy}";

        // Svojstva za unos novih datuma
        private DateTime? _newStartDate;
        public DateTime? NewStartDate
        {
            get => _newStartDate;
            set { _newStartDate = value; OnPropertyChanged(); }
        }

        private DateTime? _newEndDate;
        public DateTime? NewEndDate
        {
            get => _newEndDate;
            set { _newEndDate = value; OnPropertyChanged(); }
        }

        // Svojstvo za BlackoutDates
        public CalendarBlackoutDatesCollection BlackoutDates { get; } = new CalendarBlackoutDatesCollection(null);

        #endregion

        #region Komande
        public ICommand SendRequestCommand { get; }
        #endregion

        public RescheduleRequestViewModel(Reservation reservation)
        {
            _reservation = reservation ?? throw new ArgumentNullException(nameof(reservation));

            // Dobijanje zavisnosti
            _rescheduleRequestService = Injector.CreateInstance<IRescheduleRequestService>();
            _accommodationRepository = Injector.CreateInstance<IAccommodationRepository>();

            // Dohvatanje smeštaja
            _accommodation = _accommodationRepository.GetAll().FirstOrDefault(a => a.Id == _reservation.AccommodationId);
            if (_accommodation == null)
            {
                MessageBox.Show("Could not find accommodation details. The window will close.");
                CloseAction?.Invoke();
                return;
            }

            // Inicijalizacija komandi
            SendRequestCommand = new RelayCommand(SendRequest);

            LoadInitialData();
        }

        #region Logika

        private void LoadInitialData()
        {
            var blackoutDates = _rescheduleRequestService.GetBlackoutDatesForReschedule(_reservation);
            foreach (var date in blackoutDates)
            {
                BlackoutDates.Add(new CalendarDateRange(date));
            }
        }

        private void SendRequest(object obj)
        {
            // --- Validacija unosa ---
            if (!NewStartDate.HasValue || !NewEndDate.HasValue)
            {
                MessageBox.Show("Please select both new start and end dates.");
                return;
            }
            if (NewEndDate.Value <= NewStartDate.Value)
            {
                MessageBox.Show("End date must be after start date.");
                return;
            }

            // --- Poziv servisa ---
            try
            {
                _rescheduleRequestService.CreateRequest(_reservation, NewStartDate.Value.Date, NewEndDate.Value.Date);
                MessageBox.Show("Your request has been sent to the owner.", "Request Sent", MessageBoxButton.OK, MessageBoxImage.Information);

                // Signaliziramo uspešno zatvaranje
                // (Ovu logiku ćemo povezati u code-behind-u)
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}