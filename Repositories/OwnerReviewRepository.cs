using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Serializer;

namespace BookingApp.Repositories
{
    public class OwnerReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/guestReviewsD.csv";
        private readonly Serializer<OwnerReview> _serializer;

        //private List<OwnerReview> _reviews;

        public OwnerReviewRepository()
        {
            _serializer = new Serializer<OwnerReview>();
           // _reviews = _serializer.FromCSV(FilePath);
        }
        public List<OwnerReview> GetAll()
        {
            //return _reviews;
            return _serializer.FromCSV(FilePath);
        }
        public List<OwnerReview> GetByReservationId(int reservationId)
        {
            //return _reviews.Where(r => r.ReservationId == reservationId).ToList();
            var allReviews = GetAll();
            return allReviews.Where(r=>r.ReservationId == reservationId).ToList();
        }
        public bool HasGuestRated(int reservationId)
        {
            //return _reviews.Any(r=>r.ReservationId==reservationId);
            var allReviews = GetAll();
            return allReviews.Any(r => r.ReservationId == reservationId); ;
        }
        public OwnerReview Save(OwnerReview review)
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