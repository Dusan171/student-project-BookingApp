using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class GuestReviewRepository :IGuestReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/guestReview.csv";

        private readonly Serializer<GuestReview> _serializer;
        private List<GuestReview> _reviews;

        public GuestReviewRepository()
        {
            _serializer = new Serializer<GuestReview>();
            _reviews = _serializer.FromCSV(FilePath);
        }


        public List<GuestReview> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public GuestReview Save(GuestReview review)
        {
            review.Id = NextId();
            _reviews = _serializer.FromCSV(FilePath);
            _reviews.Add(review);
            _serializer.ToCSV(FilePath, _reviews);
            return review;
        }
        public int NextId()
        {
            _reviews = _serializer.FromCSV(FilePath);
            if (_reviews.Count < 1)
            {
                return 1;
            }
            return _reviews.Max(c => c.Id) + 1;
        }

        public void Delete(GuestReview review)
        {
            _reviews = _serializer.FromCSV(FilePath);
            GuestReview founded = _reviews.Find(c => c.Id == review.Id);
            _reviews.Remove(founded);
            _serializer.ToCSV(FilePath, _reviews);
        }

        public GuestReview Update(GuestReview review)
        {
            _reviews = _serializer.FromCSV(FilePath);
            GuestReview current = _reviews.Find(c => c.Id == review.Id);
            int index = _reviews.IndexOf(current);
            _reviews.Remove(current);
            _reviews.Insert(index, review);       
            _serializer.ToCSV(FilePath, _reviews);
            return review;
        }

        public List<GuestReview> GetByReservationId(int reservationId)
        {
            _reviews = _serializer.FromCSV(FilePath);
            return _reviews.FindAll(review => review.ReservationId == reservationId);
        }
        public List<GuestReview> GetByReservationId(Reservation reservation)
        {
            return GetByReservationId(reservation.Id);
        }
    }
}
