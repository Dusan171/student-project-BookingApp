using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
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

        public List<GuestReviewDTO> GetAllReviews()
        {
            return _repository.GetAll()
                      .Select(review => new GuestReviewDTO(review))
                      .ToList();
        }

        public GuestReviewDTO AddReview(GuestReviewDTO review)
        {
            return new GuestReviewDTO(_repository.Save(review.ToGuestReview()));
        }

        public void DeleteReview(GuestReviewDTO review)
        {
            _repository.Delete(review.ToGuestReview());
        }

        public GuestReviewDTO UpdateReview(GuestReviewDTO review)
        {
            return new GuestReviewDTO(_repository.Update(review.ToGuestReview()));
        }

        public List<GuestReviewDTO> GetReviewsByReservation(ReservationDTO reservation)
        {
            return _repository.GetByReservationId(reservation.ToReservation())
                      .Select(review => new GuestReviewDTO(review))
                      .ToList();
        }
    }
}
