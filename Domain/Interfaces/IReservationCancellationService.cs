namespace BookingApp.Domain.Interfaces
{
    public interface IReservationCancellationService
    {
        void CancelReservation(int reservationId);
    }
}