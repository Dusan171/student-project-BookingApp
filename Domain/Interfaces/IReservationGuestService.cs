using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationGuestService
    {
        List<ReservationGuestDTO> GetAllGuests();
        ReservationGuestDTO? GetGuestById(int id);
        ReservationGuestDTO AddGuest(ReservationGuestDTO guestDTO);
        ReservationGuestDTO UpdateGuest(ReservationGuestDTO guestDTO);
        void DeleteGuest(int id);

        List<ReservationGuestDTO> GetGuestsByReservation(int reservationId);
        void RemoveGuestsByReservation(int reservationId);
        List<ReservationGuestDTO> GetAppearedGuests();

        bool MarkGuestAppearance(int guestId, bool hasAppeared, int keyPointJoinedAt = -1);
        bool ValidateGuest(ReservationGuestDTO guestDTO);
       
    }
}