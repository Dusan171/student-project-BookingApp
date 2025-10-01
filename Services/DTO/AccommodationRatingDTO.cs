using System;

namespace BookingApp.Services.DTO
{
    public class AccommodationRatingDTO
    {
        public string AccommodationName { get; set; }
        public string Location { get; set; }
        public double AverageCleanlinessRating { get; set; }
        public double AverageOwnerRating { get; set; }
        public int NumberOfReviews { get; set; } 
    }
}