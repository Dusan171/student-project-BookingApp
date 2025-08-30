using System;
using BookingApp.Domain;
using BookingApp.Domain.Model;

public class TourReviewDisplay
{
    public int Id { get; set; }
    public string TourName { get; set; }
    public string TouristName { get; set; }
    public string KeyPointJoinedAt { get; set; }
    public int GuideKnowledgeRating { get; set; }
    public int GuideLanguageRating { get; set; }
    public int TourInterestRating { get; set; }
    public string Comment { get; set; }
    public bool IsValid { get; set; } = true;

    // referenca na original
    public TourReview OriginalReview { get; set; }

    public TourReviewDisplay(
        int id,
        string tourName,
        bool isvalid,
        string touristName,
        string keyPointJoinedAt,
        int guideKnowledgeRating,
        int guideLanguageRating,
        int tourInterestRating,
        string comment,
        TourReview originalReview)
    {
        Id = id;
        TourName = tourName;
        TouristName = touristName;
        KeyPointJoinedAt = keyPointJoinedAt;
        GuideKnowledgeRating = guideKnowledgeRating;
        GuideLanguageRating = guideLanguageRating;
        TourInterestRating = tourInterestRating;
        Comment = comment;
        IsValid = isvalid;   
        OriginalReview = originalReview;
    }
}
