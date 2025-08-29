using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BookingApp.Domain
{
    public class TourReview : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int GuideKnowledgeRating { get; set; } // 1-5 znanje vodiča
        public int GuideLanguageRating { get; set; } // 1-5 jezik vodiča
        public int TourInterestRating { get; set; } // 1-5 zanimljivost ture
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
            _isValid = true;
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