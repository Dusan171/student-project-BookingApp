using BookingApp.Services.DTO;
using BookingApp.Domain.Model;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationOrchestratorService
    {
        ReservationOrchestrationResultDTO ExecuteReservation(ReservationDTO reservationDto);
        ReservationOrchestrationResultDTO PrepareAndExecuteReservation(AnywhereSearchResultDTO offer, string guestsNumberInput, int currentUserId);
    }
}