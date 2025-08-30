using System;

namespace BookingApp.Services.DTO 
{
    public class AccommodationReviewDetailsDTO
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }

        public int CleanlinessRating { get; set; }
        public int OwnerRating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}