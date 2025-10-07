using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class AccommodationReservationViewModel : ViewModelBase
    {
        private readonly AccommodationDetailsDTO _accommodationDetails;
        private readonly IReservationCreationService _reservationCreationService; 
        private readonly IReservationOrchestratorService _orchestratorService;

        public AccommodationDetailsDTO AccommodationDetails => _accommodationDetails;
        public Action CloseAction { get; set; }

        #region Properties & Commands
        private DateTime? _startDate;
        public DateTime? StartDate { get; set; }

        private DateTime? _endDate;
        public DateTime? EndDate { get; set; }

        private string _guestsNumber;
        public string GuestsNumber { get; set; }

        public List<DateTime> OccupiedDates { get; private set; }
        public ICommand ReserveCommand { get; }
        #endregion

        public AccommodationReservationViewModel(AccommodationDetailsDTO accommodationDetails)
        {
            _accommodationDetails = accommodationDetails;
            _reservationCreationService = Injector.CreateInstance<IReservationCreationService>();
            _orchestratorService = Injector.CreateInstance<IReservationOrchestratorService>();

            ReserveCommand = new RelayCommand(Reserve, CanReserve);
            LoadOccupiedDates();
        }

        private void LoadOccupiedDates()
        {
            OccupiedDates = _reservationCreationService.GetOccupiedDatesForAccommodation(_accommodationDetails.Id);
        }

        private bool CanReserve(object obj)
        {
            return StartDate.HasValue && EndDate.HasValue && !string.IsNullOrWhiteSpace(GuestsNumber);
        }

        private void Reserve(object obj)
        {
            if (!ValidateInput(out int guestNumber)) return;

            var reservationDto = CreateReservationDTO(guestNumber);
            var result = _orchestratorService.ExecuteReservation(reservationDto);

            HandleOrchestrationResult(result);
        }

        #region Private Helper Methods

        private bool ValidateInput(out int parsedGuestNumber)
        {
            if (EndDate.Value <= StartDate.Value)
            {
                MessageBox.Show("End date must be after the start date.");
                parsedGuestNumber = 0;
                return false;
            }
            if (!int.TryParse(GuestsNumber, out parsedGuestNumber) || parsedGuestNumber <= 0)
            {
                MessageBox.Show("Please enter a valid number of guests.");
                return false;
            }
            return true;
        }

        private ReservationDTO CreateReservationDTO(int guestNumber)
        {
            return new ReservationDTO
            {
                AccommodationId = _accommodationDetails.Id,
                GuestId = Session.CurrentUser.Id,
                StartDate = StartDate.Value.Date,
                EndDate = EndDate.Value.Date,
                GuestsNumber = guestNumber
            };
        }

        private void HandleOrchestrationResult(ReservationOrchestrationResultDTO result)
        {
            if (!string.IsNullOrEmpty(result.Message))
            {
                MessageBox.Show(result.Message);
            }

            if (result.ShouldCloseView)
            {
                CloseAction?.Invoke();
            }
            else if (result.NewStartDate.HasValue && result.NewEndDate.HasValue)
            {
                this.StartDate = result.NewStartDate;
                this.EndDate = result.NewEndDate;
                OnPropertyChanged(nameof(StartDate));
                OnPropertyChanged(nameof(EndDate));
            }
        }

        #endregion
    }
}