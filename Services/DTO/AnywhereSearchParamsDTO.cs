using System;

namespace BookingApp.Services.DTO
{
    public class AnywhereSearchParamsDTO
    {
        public int GuestsNumber { get; set; }
        public int StayDuration { get; set; }
        public DateTime? StartDate { get; set; } // Opciono
        public DateTime? EndDate { get; set; }   // Opciono
    }
}