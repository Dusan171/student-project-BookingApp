using BookingApp.Domain;
using System;

namespace BookingApp.Services.DTO
{
    public class ReservationDetailsDTO
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestsNumber { get; set; }
        public string AccommodationName { get; set; }
        public string RequestStatusText { get; set; }
        public string OwnerComment { get; set; }
        public bool IsRescheduleEnabled { get; set; }
        public Reservation OriginalReservation { get; set; } 
        public bool IsRatingEnabled { get; set; }
        public bool IsGuestReviewVisible { get; set; }
    }
}