using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BookingApp.Domain.Model
{
    public class TourReview : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int ReservationId { get; set; }   // novo polje
        public int GuideKnowledge { get; set; }  // 1-5
        public int GuideLanguage { get; set; }   // 1-5
        public int TourInterest { get; set; }    // 1-5
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        private bool _isValid = true;
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

        public List<string> ImagePaths { get; set; } = new List<string>(); // Putanje do slika

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
            ReviewDate = DateTime.Now;
            _isValid = true;
        }

        public double GetAverageRating()
        {
            return (GuideKnowledge + GuideLanguage + TourInterest) / 3.0;
        }

        public string GetRatingText()
        {
            return $"Znanje vodiča: {GuideKnowledge}/5, Jezik vodiča: {GuideLanguage}/5, Zanimljivost: {TourInterest}/5";
        }

        public string GetFormattedDate()
        {
            return ReviewDate.ToString("dd.MM.yyyy HH:mm");
        }

        // INotifyPropertyChanged implementacija
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
