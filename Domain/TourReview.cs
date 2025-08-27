using System;
using System.Collections.Generic;

namespace BookingApp.Domain
{
    public class TourReview
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int GuideKnowledgeRating { get; set; } // 1-5 znanje vodiča
        public int GuideLanguageRating { get; set; } // 1-5 jezik vodiča
        public int TourInterestRating { get; set; } // 1-5 zanimljivost ture
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public bool IsValid { get; set; } = true;
        public List<string> ImagePaths { get; set; } = new List<string>(); // Putanje do slika

        // Navigation properties
        public Tour? Tour { get; set; }
        public User? Tourist { get; set; }

        public TourReview()
        {
            ReviewDate = DateTime.Now;
        }

        public TourReview(int tourId, int touristId, int guideKnowledgeRating, int guideLanguageRating, int tourInterestRating, string comment)
        {
            TourId = tourId;
            TouristId = touristId;
            GuideKnowledgeRating = guideKnowledgeRating;
            GuideLanguageRating = guideLanguageRating;
            TourInterestRating = tourInterestRating;
            Comment = comment;
            ReviewDate = DateTime.Now;
            IsValid = true;
        }

        public double GetAverageRating()
        {
            return (GuideKnowledgeRating + GuideLanguageRating + TourInterestRating) / 3.0;
        }

        public string GetRatingText()
        {
            return $"Znanje vodiča: {GuideKnowledgeRating}/5, Jezik vodiča: {GuideLanguageRating}/5, Zanimljivost: {TourInterestRating}/5";
        }

        public string GetFormattedDate()
        {
            return ReviewDate.ToString("dd.MM.yyyy HH:mm");
        }
    }
}