using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TourReviewRepository : ITourReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/tourreviews.csv";
        private List<TourReview> _reviews;
        private int _nextId;

        public TourReviewRepository()
        {
            _reviews = LoadFromFile();
            _nextId = _reviews.Count > 0 ? _reviews.Max(r => r.Id) + 1 : 1;
        }

        private List<TourReview> LoadFromFile()
        {
            var reviews = new List<TourReview>();
            if (!File.Exists(FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                return reviews;
            }

            foreach (var line in File.ReadAllLines(FilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('|');
                if (parts.Length < 10) continue;

                try
                {
                    var review = new TourReview
                    {
                        Id = int.Parse(parts[0]),
                        TourId = int.Parse(parts[1]),
                        TouristId = int.Parse(parts[2]),
                        ReservationId = int.Parse(parts[3]),
                        GuideKnowledge = int.Parse(parts[4]),
                        GuideLanguage = int.Parse(parts[5]),
                        TourInterest = int.Parse(parts[6]),
                        Comment = parts[7],
                        ReviewDate = DateTime.ParseExact(parts[8], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        IsValid = bool.Parse(parts[9]),
                        ImagePaths = parts.Length > 10 && !string.IsNullOrWhiteSpace(parts[10])
                            ? parts[10].Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                            : new List<string>()
                    };
                    reviews.Add(review);
                }
                catch { /* ignorisati nevalidan red */ }
            }

            return reviews;
        }

        public void SaveToFile()
        {
            var lines = _reviews.Select(r =>
                $"{r.Id}|{r.TourId}|{r.TouristId}|{r.ReservationId}|{r.GuideKnowledge}|{r.GuideLanguage}|{r.TourInterest}|{r.Comment}|{r.ReviewDate:yyyy-MM-dd HH:mm:ss}|{r.IsValid}|{string.Join(";", r.ImagePaths)}");

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllLines(FilePath, lines);
        }

        public List<TourReview> GetAll() => _reviews.ToList();
        public TourReview? GetById(int id) => _reviews.FirstOrDefault(r => r.Id == id);
        public List<TourReview> GetByTourId(int tourId) => _reviews.Where(r => r.TourId == tourId && r.IsValid).ToList();
        public List<TourReview> GetByTouristId(int touristId) => _reviews.Where(r => r.TouristId == touristId && r.IsValid).ToList();
        public bool HasReview(int touristId, int tourId) => _reviews.Any(r => r.TouristId == touristId && r.TourId == tourId && r.IsValid);

        public void AddReview(TourReview review)
        {
            review.Id = _nextId++;
            review.ReviewDate = DateTime.Now;
            _reviews.Add(review);
            SaveToFile();
        }


        public void UpdateReview(TourReview review)
        {
            var existing = _reviews.FirstOrDefault(r => r.Id == review.Id);
            if (existing != null)
            {
                existing.GuideKnowledge = review.GuideKnowledge;
                existing.GuideLanguage = review.GuideLanguage;
                existing.TourInterest = review.TourInterest;
                existing.Comment = review.Comment;
                existing.ImagePaths = review.ImagePaths;
                existing.IsValid = review.IsValid;
                SaveToFile();
            }
        }

        public void DeleteReview(int id)
        {
            var review = _reviews.FirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                review.IsValid = false;
                SaveToFile();
            }
        }

        public double GetAverageRatingForTour(int tourId)
        {
            var tourReviews = GetByTourId(tourId);
            return tourReviews.Count > 0 ? tourReviews.Average(r => (r.GuideKnowledge + r.GuideLanguage + r.TourInterest) / 3.0) : 0.0;
        }
    }
}
