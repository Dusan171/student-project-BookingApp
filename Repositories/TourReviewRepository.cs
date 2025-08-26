using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BookingApp.Domain;

namespace BookingApp.Repositories
{
    public class TourReviewRepository
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

            var lines = File.ReadAllLines(FilePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('|');
                if (parts.Length >= 8) 
                {
                    try
                    {
                        var review = new TourReview
                        {
                            Id = int.Parse(parts[0]),
                            TourId = int.Parse(parts[1]),
                            TouristId = int.Parse(parts[2]),
                            GuideKnowledgeRating = int.Parse(parts[3]),
                            GuideLanguageRating = int.Parse(parts[4]),
                            TourInterestRating = int.Parse(parts[5]),
                            Comment = parts[6],
                            ReviewDate = DateTime.ParseExact(parts[7], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            IsValid = parts.Length > 8 ? bool.Parse(parts[8]) : true
                        };

                        
                        if (parts.Length > 9 && !string.IsNullOrWhiteSpace(parts[9]))
                        {
                            review.ImagePaths = parts[9].Split(';').Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                        }

                        reviews.Add(review);
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine($"Error parsing review line: {line}, Error: {ex.Message}");
                    }
                }
            }

            return reviews;
        }

        private void SaveToFile()
        {
            try
            {
                var lines = _reviews.Select(r =>
                    $"{r.Id}|{r.TourId}|{r.TouristId}|{r.GuideKnowledgeRating}|{r.GuideLanguageRating}|{r.TourInterestRating}|{r.Comment}|{r.ReviewDate:yyyy-MM-dd HH:mm:ss}|{r.IsValid}|{string.Join(";", r.ImagePaths)}");

                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllLines(FilePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving reviews to file: {ex.Message}");
            }
        }

        public List<TourReview> GetAll()
        {
            return _reviews.ToList();
        }

        public TourReview? GetById(int id)
        {
            return _reviews.FirstOrDefault(r => r.Id == id);
        }

        public List<TourReview> GetByTourId(int tourId)
        {
            return _reviews.Where(r => r.TourId == tourId && r.IsValid).ToList();
        }

        public List<TourReview> GetByTouristId(int touristId)
        {
            return _reviews.Where(r => r.TouristId == touristId && r.IsValid).ToList();
        }

        public bool HasTouristReviewedTour(int touristId, int tourId)
        {
            return _reviews.Any(r => r.TouristId == touristId && r.TourId == tourId && r.IsValid);
        }

        public void Add(TourReview review)
        {
            review.Id = _nextId++;
            review.ReviewDate = DateTime.Now;
            _reviews.Add(review);
            SaveToFile();
        }

        public void Update(TourReview review)
        {
            var existingReview = _reviews.FirstOrDefault(r => r.Id == review.Id);
            if (existingReview != null)
            {
                existingReview.GuideKnowledgeRating = review.GuideKnowledgeRating;
                existingReview.GuideLanguageRating = review.GuideLanguageRating;
                existingReview.TourInterestRating = review.TourInterestRating;
                existingReview.Comment = review.Comment;
                existingReview.ImagePaths = review.ImagePaths;
                existingReview.IsValid = review.IsValid;
                SaveToFile();
            }
        }

        public void Delete(int id)
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
            return tourReviews.Count > 0 ? tourReviews.Average(r => r.GetAverageRating()) : 0.0;
        }

        public int GetReviewCountForTour(int tourId)
        {
            return GetByTourId(tourId).Count;
        }

        public List<TourReview> GetReviewsForCompletedTours(int touristId)
        {
            var tourRepository = new TourRepository();
            var reservationRepository = new TourReservationRepository();

            // Pronađi sve završene rezervacije korisnika
            var completedReservations = reservationRepository.GetReservationsByTourist(touristId)
                ?.Where(r => r.Status == TourReservationStatus.COMPLETED)
                .ToList() ?? new List<TourReservation>();

            var completedTourIds = completedReservations.Select(r => r.TourId).Distinct().ToList();

            // Vrati recenzije samo za završene ture
            return _reviews.Where(r => r.TouristId == touristId &&
                                      completedTourIds.Contains(r.TourId) &&
                                      r.IsValid).ToList();
        }
    }
}