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
    public class AccommodationReservationViewModel : ViewModelBase
    {
        private readonly Accommodation _accommodation;
        private readonly IReservationService _reservationService;
        private readonly IOccupiedDateRepository _occupiedDateRepository;

        // Akcija za zatvaranje prozora, koju će View postaviti
        public Action CloseAction { get; set; }

        #region Svojstva za povezivanje (Binding)

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        private string _guestsNumber;
        public string GuestsNumber
        {
            get => _guestsNumber;
            set { _guestsNumber = value; OnPropertyChanged(); }
        }

        // Svojstvo za povezivanje sa BlackoutDates u XAML-u
        public CalendarBlackoutDatesCollection BlackoutDates { get; } = new CalendarBlackoutDatesCollection(null);

        #endregion

        #region Komande
        public ICommand ReserveCommand { get; }
        #endregion

        public AccommodationReservationViewModel(Accommodation accommodation)
        {
            _accommodation = accommodation ?? throw new ArgumentNullException(nameof(accommodation));

            // Dobijanje zavisnosti
            _reservationService = Injector.CreateInstance<IReservationService>();
            _occupiedDateRepository = Injector.CreateInstance<IOccupiedDateRepository>();

            // Inicijalizacija komandi
            ReserveCommand = new RelayCommand(Reserve);

            LoadOccupiedDates();
        }

        #region Logika

        private void LoadOccupiedDates()
        {
            var occupiedDates = _occupiedDateRepository.GetByAccommodationId(_accommodation.Id);
            foreach (var date in occupiedDates)
            {
                BlackoutDates.Add(new CalendarDateRange(date.Date));
            }
        }

        private void Reserve(object obj)
        {
            // --- Validacija unosa ---
            if (!StartDate.HasValue)
            {
                MessageBox.Show("Please select a start date.");
                return;
            }
            if (!EndDate.HasValue)
            {
                MessageBox.Show("Please select an end date.");
                return;
            }
            if (EndDate.Value <= StartDate.Value)
            {
                MessageBox.Show("End date must be after the start date.");
                return;
            }
            if (!int.TryParse(GuestsNumber, out int guestNumber) || guestNumber <= 0)
            {
                MessageBox.Show("Please enter a valid number of guests.");
                return;
            }

            // --- Poziv servisa ---
            try
            {
                _reservationService.Create(_accommodation, StartDate.Value, EndDate.Value, guestNumber);
                MessageBox.Show("Reservation successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Pozivamo akciju da zatvori prozor
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Reservation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}