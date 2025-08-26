using System.Collections.Generic;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationDisplayService
    {
        public List<ReservationDetailsDTO> GetReservationsForGuest(int guestId);
    }
}
