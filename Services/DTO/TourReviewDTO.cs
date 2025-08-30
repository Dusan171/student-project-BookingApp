using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services.DTO
{
    public class TourReviewDTO
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int ReservationId { get; set; } // novo polje
        public int GuideKnowledge { get; set; }
        public int GuideLanguage { get; set; }
        public int TourInterest { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public bool IsValid { get; set; } = true;
        public List<string> ImagePaths { get; set; } = new List<string>();

        // Optional details for UI
        public string? TourName { get; set; }
        public string? TouristName { get; set; }
        public double AverageRating => (GuideKnowledge + GuideLanguage + TourInterest) / 3.0;
        public string RatingText => $"Znanje vodiča: {GuideKnowledge}/5, Jezik vodiča: {GuideLanguage}/5, Zanimljivost: {TourInterest}/5";
        public string FormattedDate => ReviewDate.ToString("dd.MM.yyyy HH:mm");

        public TourReviewDTO() { }

        public TourReviewDTO(TourReview review)
        {
            if (review == null) return;

            Id = review.Id;
            TourId = review.TourId;
            TouristId = review.TouristId;
            ReservationId = review.ReservationId; // dodato
            GuideKnowledge = review.GuideKnowledge;
            GuideLanguage = review.GuideLanguage;
            TourInterest = review.TourInterest;
            Comment = review.Comment;
            ReviewDate = review.ReviewDate;
            IsValid = review.IsValid;
            ImagePaths = review.ImagePaths?.ToList() ?? new List<string>();
            TourName = review.Tour?.Name;
            TouristName = review.Tourist?.FirstName + " " + review.Tourist?.LastName;
        }

        public TourReview ToDomain()
        {
            return new TourReview(TourId, TouristId, ReservationId, // dodato
                                  GuideKnowledge, GuideLanguage, TourInterest,
                                  Comment)
            {
                Id = Id,
                ReviewDate = ReviewDate,
                IsValid = IsValid,
                ImagePaths = ImagePaths.ToList()
            };
        }
    }
}
