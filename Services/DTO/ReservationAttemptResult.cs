using System.Collections.Generic;

namespace BookingApp.Services.DTO
{
    public class ReservationAttemptResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public ReservationDTO CreatedReservation { get; set; }

        public List<DateRange> SuggestedRanges { get; set; }

        public ReservationAttemptResult()
        {
            SuggestedRanges = new List<DateRange>();
        }
    }
}