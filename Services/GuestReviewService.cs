using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Repositories;
using System.ComponentModel.DataAnnotations;
using System.Windows;

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
        public void AddReview(GuestReviewDTO reviewDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reviewDTO.Comment))
                {
                    throw new ValidationException("Must enter explanation.");
                }
                if (IsRatingInvalid(reviewDTO.CleanlinessRating) || IsRatingInvalid(reviewDTO.RuleRespectingRating))
                {
                    throw new ValidationException("Enter number between 1 and 5.");
                }

                GuestReview review = reviewDTO.ToGuestReview();
                _repository.Save(review);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool IsRatingInvalid(int rating)
        {
            return rating < 1 || rating > 5;
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
            return _repository.GetByReservationId(reservation.ToReservation().Id)
                      .Select(review => new GuestReviewDTO(review)).ToList();
        }
        public GuestReviewDTO GetReviewForReservation(int reservationId)
        {
            var reviewModel = _repository.GetByReservationId(reservationId).FirstOrDefault();

            if (reviewModel == null)
            {
                return null;
            }

            return new GuestReviewDTO(reviewModel);
        }
    }
}