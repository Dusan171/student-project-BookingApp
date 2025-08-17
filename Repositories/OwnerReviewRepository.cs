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

        private List<OwnerReview> _reviews;

        public OwnerReviewRepository()
        {
            _serializer = new Serializer<OwnerReview>();
            _reviews = _serializer.FromCSV(FilePath);
        }
        public List<OwnerReview> GetAll()
        {
            return _reviews;
        }
        public List<OwnerReview> GetByReservationId(int reservationId)
        {
            return _reviews.Where(r => r.ReservationId == reservationId).ToList();
        }
        public bool HasGuestRated(int reservationId)
        {
            return _reviews.Any(r=>r.ReservationId==reservationId);
        }
        public OwnerReview Save(OwnerReview reviewD)
        {
            reviewD.Id = NextId();

            _reviews.Add(reviewD);
            _serializer.ToCSV(FilePath, _reviews);
            return reviewD;
        }
        public int NextId()
        {
            return _reviews.Count == 0 ? 1 : _reviews.Max(r => r.Id) + 1;
        }
    }
}