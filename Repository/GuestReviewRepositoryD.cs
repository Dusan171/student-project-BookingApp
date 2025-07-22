
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    public class GuestReviewRepositoryD
    {
        private const string FilePath = "../../../Resources/Data/guestReviewsD.csv";
        private readonly Serializer<GuestReviewD> _serializer;

        private List<GuestReviewD> _reviews;

        public GuestReviewRepositoryD()
        {
            _serializer = new Serializer<GuestReviewD>();
            _reviews = _serializer.FromCSV(FilePath);
        }
        public List<GuestReviewD> GetAll()
        {
            return _reviews;
        }
        public List<GuestReviewD> GetByReservationId(int reservationId)
        {
            return _reviews.Where(r => r.ReservationId == reservationId).ToList();
        }
        public GuestReviewD Save(GuestReviewD reviewD)
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