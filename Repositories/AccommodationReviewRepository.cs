using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Repositories
{
    public class AccommodationReviewRepository : IAccommodationReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/guestReviewsD.csv";
        private readonly Serializer<AccommodationReview> _serializer;

        public AccommodationReviewRepository()
        {
            _serializer = new Serializer<AccommodationReview>();
        }
        public List<AccommodationReview> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }
        public List<AccommodationReview> GetByReservationId(int reservationId)
        {
            var allReviews = GetAll();
            return allReviews.Where(r => r.ReservationId == reservationId).ToList();
        }
        public bool HasGuestRated(int reservationId)
        {
            var allReviews = GetAll();
            return allReviews.Any(r => r.ReservationId == reservationId);
        }
        public AccommodationReview Save(AccommodationReview review)
        {
            var allReviews = GetAll();
            review.Id = NextId();
            allReviews.Add(review);
            _serializer.ToCSV(FilePath, allReviews);
            return review;
        }
        public int NextId()
        {
            var allReviews = GetAll();
            return allReviews.Any() ? allReviews.Max(r => r.Id) + 1 : 1;
        }
    }
}