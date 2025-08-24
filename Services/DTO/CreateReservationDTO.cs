using System;

namespace BookingApp.Services.DTOs
{
    public class CreateReservationDTO
    {
        public int AccommodationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestsNumber { get; set; }
    }
}