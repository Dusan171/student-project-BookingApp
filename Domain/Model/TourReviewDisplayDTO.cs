using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Domain;
using BookingApp.Domain.Model;

public class TourReviewDisplayDTO : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string TourName { get; set; }
    public string TouristName { get; set; }
    public string KeyPointJoinedAt { get; set; }
    public int GuideKnowledgeRating { get; set; }
    public int GuideLanguageRating { get; set; }
    public int TourInterestRating { get; set; }
    public string Comment { get; set; }
    public bool HasImages => ImagePaths != null && ImagePaths.Any();

    private bool _isValid;
    public bool IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid != value)
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }
    }

    // referenca na original
    public TourReview OriginalReview { get; set; }
    public List<string> ImagePaths { get; set; }

    public TourReviewDisplayDTO(int id, string tourName, bool isValid, string touristName, string keyPointJoinedAt,
                                int guideKnowledgeRating, int guideLanguageRating, int tourInterestRating,
                                string comment, TourReview originalReview, List<string> images = null)
    {
        Id = id;
        TourName = tourName;
        IsValid = isValid;
        TouristName = touristName;
        KeyPointJoinedAt = keyPointJoinedAt;
        GuideKnowledgeRating = guideKnowledgeRating;
        GuideLanguageRating = guideLanguageRating;
        TourInterestRating = tourInterestRating;
        Comment = comment;
        OriginalReview = originalReview;
        ImagePaths = images ?? new List<string>();
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
