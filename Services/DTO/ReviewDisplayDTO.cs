using System;

namespace BookingApp.Services.DTO
{
    public class ReviewDisplayDTO
    {
        public int ReservationId { get; set; }
        public string GuestName { get; set; }
        public string AccommodationName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        // Host → Guest ratings
        public int? CleanlinessRating { get; set; }
        public int? RuleRespectingRating { get; set; }

        // Guest → Host ratings
        public int? OwnerRating { get; set; }
        public string Comment { get; set; }
   
        public string ImagePaths { get; set; }
        public int ImageCount { get; set; }
        public string FirstImagePath { get; set; }
        public bool HasImages => !string.IsNullOrEmpty(FirstImagePath);

      
        public double AverageRating
        {
            get
            {
                if (CleanlinessRating.HasValue && RuleRespectingRating.HasValue)
                    return (CleanlinessRating.Value + RuleRespectingRating.Value) / 2.0;
                if (CleanlinessRating.HasValue && OwnerRating.HasValue)
                    return (CleanlinessRating.Value + OwnerRating.Value) / 2.0;
                return 0;
            }
        }
    }
}