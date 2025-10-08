using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Presentation.View.Guest;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class ReservationOrchestratorService : IReservationOrchestratorService
    {
        private readonly IReservationCreationService _creationService;

        public ReservationOrchestratorService(IReservationCreationService creationService)
        {
            _creationService = creationService;
        }

        public ReservationOrchestrationResultDTO PrepareAndExecuteReservation(AnywhereSearchResultDTO offer, string guestsNumberInput, int currentUserId)
        {
            if (!TryParseReservationData(offer, guestsNumberInput, currentUserId, out var reservationDto))
            {
                return HandleFailure("Invalid input for guests or date range. Please check your entries.");
            }

            return ExecuteReservationInternal(reservationDto);
        }
        public ReservationOrchestrationResultDTO ExecuteReservation(ReservationDTO reservationDto)
        {
            var result = _creationService.AttemptReservation(reservationDto);

            if (result.IsSuccess)
            {
                return HandleSuccess();
            }

            return result.SuggestedRanges.Any()
                ? HandleSuggestions(result.SuggestedRanges)
                : HandleFailure(result.ErrorMessage);
        }

        private bool TryParseReservationData(AnywhereSearchResultDTO offer, string guestsNumberInput, int currentUserId, out ReservationDTO reservationDto)
        {
            reservationDto = null;

            var dateParts = offer.OfferedDateRange.Split(" - ");
            const string format = "dd.MM.yyyy";
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (dateParts.Length != 2 ||
                !DateTime.TryParseExact(dateParts[0], format, culture, System.Globalization.DateTimeStyles.None, out var startDate) ||
                !DateTime.TryParseExact(dateParts[1], format, culture, System.Globalization.DateTimeStyles.None, out var endDate) ||
                !int.TryParse(guestsNumberInput, out var guests))
            {
                return false; 
            }

            reservationDto = new ReservationDTO
            {
                AccommodationId = offer.Accommodation.Id,
                GuestId = currentUserId,
                StartDate = startDate,
                EndDate = endDate,
                GuestsNumber = guests
            };
            return true;
        }

        private ReservationOrchestrationResultDTO ExecuteReservationInternal(ReservationDTO reservationDto)
        {
            var result = _creationService.AttemptReservation(reservationDto);

            if (result.IsSuccess)
            {
                return HandleSuccess();
            }

            return result.SuggestedRanges.Any()
                ? HandleSuggestions(result.SuggestedRanges)
                : HandleFailure(result.ErrorMessage);
        }

        private ReservationOrchestrationResultDTO HandleSuccess()
        {
            return new ReservationOrchestrationResultDTO
            {
                WasSuccessful = true,
                ShouldCloseView = true,
                Message = "Reservation successful!"
            };
        }

        private ReservationOrchestrationResultDTO HandleFailure(string errorMessage)
        {
            return new ReservationOrchestrationResultDTO
            {
                WasSuccessful = false,
                ShouldCloseView = false,
                Message = errorMessage
            };
        }

        private ReservationOrchestrationResultDTO HandleSuggestions(List<DateRange> suggestions)
        {
            var suggestionsWindow = new SuggestedDatesView(suggestions);
            suggestionsWindow.ShowDialog();

            if (suggestionsWindow.DataContext is SuggestedDatesViewModel vm && vm.IsConfirmed)
            {
                var chosen = vm.SelectedDateRange;
                return new ReservationOrchestrationResultDTO
                {
                    WasSuccessful = false,
                    ShouldCloseView = false,
                    Message = $"New dates ({chosen.StartDate:dd.MM.yyyy} - {chosen.EndDate:dd.MM.yyyy}) have been selected. Please click 'Reserve' again to confirm.",
                    NewStartDate = chosen.StartDate,
                    NewEndDate = chosen.EndDate
                };
            }

            return new ReservationOrchestrationResultDTO { WasSuccessful = false, ShouldCloseView = false, Message = "Reservation was not made." };
        }
    }
}