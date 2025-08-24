using System;

namespace BookingApp.Services.DTOs
{
    public class CreateRescheduleRequestDTO
    {
        public int ReservationId { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }

        // DTO ne mora da ima To...() metodu,
        // logika kreiranja domenskog modela može biti u servisu.
    }
}
