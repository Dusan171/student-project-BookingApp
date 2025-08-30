using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Model
{
    public class TourReview
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int ReservationId { get; set; }   // novo polje
        public int GuideKnowledge { get; set; }  // 1-5
        public int GuideLanguage { get; set; }   // 1-5
        public int TourInterest { get; set; }    // 1-5
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsValid { get; set; } = true;
        public List<string> ImagePaths { get; set; } = new List<string>();

        // Navigation properties
        public Tour? Tour { get; set; }
        public User? Tourist { get; set; }

        public TourReview() { }

        public TourReview(int tourId, int touristId, int reservationId,
                          int guideKnowledge, int guideLanguage, int tourInterest,
                          string comment)
        {
            TourId = tourId;
            TouristId = touristId;
            ReservationId = reservationId;
            GuideKnowledge = guideKnowledge;
            GuideLanguage = guideLanguage;
            TourInterest = tourInterest;
            Comment = comment;
            Date = DateTime.Now;
        }
    }
}
