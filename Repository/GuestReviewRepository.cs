using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    public class GuestReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/guestReviews.csv";
        private readonly Serializer<GuestReview> _serializer;
        private List<GuestReview> _reviews;

        public GuestReviewRepository()
        {
            _serializer = new Serializer<GuestReview>();
            _reviews = _serializer.FromCSV(FilePath);
        }
        public List<GuestReview> GetAll()
        {
            return _reviews;
        }
        public List<GuestReview> GetByReservationId(int reservationId)
        {
            return _reviews.Where(r => r.ReservationId == reservationId).ToList();
        }
        public GuestReview Save(GuestReview review)
        {
            review.Id = NextId();
            _reviews.Add(review);
            _serializer.ToCSV(FilePath, _reviews);
            return review;
        }
        public int NextId()
        {
            return _reviews.Count == 0 ? 1 : _reviews.Max(r => r.Id) + 1;
        }
    }
}
