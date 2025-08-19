using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class GuestReviewService : IGuestReviewService
    {
        private readonly IGuestReviewRepository _repository;

        public GuestReviewService(IGuestReviewRepository repository)
        {
            _repository = repository;
        }

        public List<GuestReview> GetAllReviews()
        {
            return _repository.GetAll();
        }

        public GuestReview AddReview(GuestReview review)
        {
            return _repository.Save(review);
        }

        public void DeleteReview(GuestReview review)
        {
            _repository.Delete(review);
        }

        public GuestReview UpdateReview(GuestReview review)
        {
            return _repository.Update(review);
        }

        public List<GuestReview> GetReviewsByReservation(Reservation reservation)
        {
            return _repository.GetByReservationId(reservation);
        }
    }
}
