using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTOs;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel
{
    public class AccommodationReservationViewModel : ViewModelBase
    {
        private readonly Accommodation _accommodation;
        private readonly IReservationService _reservationService;
        
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
        public List<DateTime> OccupiedDates { get; set; }

        #endregion

        #region Komande
        public ICommand ReserveCommand { get; }
        #endregion

        public AccommodationReservationViewModel(Accommodation accommodation)
        {
            _accommodation = accommodation ?? throw new ArgumentNullException(nameof(accommodation));

            // Dobijanje zavisnosti
            _reservationService = Injector.CreateInstance<IReservationService>();

            // Inicijalizacija komandi
            ReserveCommand = new RelayCommand(Reserve);

            LoadOccupiedDates();
        }

        #region Logika

        private void LoadOccupiedDates()
        {
            OccupiedDates = _reservationService.GetOccupiedDatesForAccommodation(_accommodation.Id);
        }

        private void Reserve(object obj)
        {
            // --- PROMENA #1: Koristimo pomoćnu metodu za validaciju ---
            // 'out int guestNumber' nam omogućava da dobijemo parsiranu vrednost iz metode
            if (!IsInputValid(out int guestNumber))
            {
                return; // Ako nije validno, prekini izvršavanje
            }

            try
            {
                // --- Logika ostaje ista, ali je metoda sada kraća ---
                var reservationDto = new CreateReservationDTO
                {
                    AccommodationId = _accommodation.Id,
                    StartDate = StartDate.Value.Date,
                    EndDate = EndDate.Value.Date,
                    GuestsNumber = guestNumber // Koristimo vrednost dobijenu iz validacije
                };
                _reservationService.Create(reservationDto);

                MessageBox.Show("Reservation successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Reservation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool IsInputValid(out int parsedGuestNumber)
        {
            parsedGuestNumber = 0; // Inicijalizujemo 'out' parametar

            if (!StartDate.HasValue)
            {
                MessageBox.Show("Please select a start date.");
                return false;
            }
            if (!EndDate.HasValue)
            {
                MessageBox.Show("Please select an end date.");
                return false;
            }
            if (EndDate.Value <= StartDate.Value)
            {
                MessageBox.Show("End date must be after the start date.");
                return false;
            }
            if (!int.TryParse(GuestsNumber, out parsedGuestNumber) || parsedGuestNumber <= 0)
            {
                MessageBox.Show("Please enter a valid number of guests.");
                return false;
            }

            return true; // Sve provere su prošle
        }

        #endregion
    }
}