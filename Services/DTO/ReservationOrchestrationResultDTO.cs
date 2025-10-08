using System;

namespace BookingApp.Services.DTO
{
    public class ReservationOrchestrationResultDTO
    {
        public bool WasSuccessful { get; set; }
        public bool ShouldCloseView { get; set; }
        public string Message { get; set; }
        public DateTime? NewStartDate { get; set; } 
        public DateTime? NewEndDate { get; set; }   
    }
}