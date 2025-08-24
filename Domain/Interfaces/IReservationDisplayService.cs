using System.Collections.Generic;
using BookingApp.Services.DTOs;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationDisplayService
    {
        public List<ReservationDetailsDTO> GetReservationsForGuest(int guestId);
    }
}
