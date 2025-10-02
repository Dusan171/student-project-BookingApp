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
        private readonly IReservationService _reservationService;
        private readonly IUserService _userService;
        private readonly IAccommodationService _accommodationService;

        public GuestReviewService(IGuestReviewRepository repository,
                                 IReservationService reservationService,
                                 IUserService userService,
                                 IAccommodationService accommodationService)
        {
            _repository = repository;
            _reservationService = reservationService;
            _userService = userService;
            _accommodationService = accommodationService;
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
        public GuestRatingDetailsDTO GetRatingDetailsForReservation(int reservationId)
        {
            try
            {
                var reservation = _reservationService.GetById(reservationId);
                if (reservation == null)
                {
                    throw new Exception($"Reservation with ID {reservationId} not found.");
                }

                var guest = _userService.GetUserById(reservation.GuestId);
                var accommodation = _accommodationService.GetAccommodationById(reservation.AccommodationId);

                var guestName = "Unknown Guest";
                if (guest != null)
                {
                    guestName = $"{guest.FirstName} {guest.LastName}".Trim();
                    if (string.IsNullOrWhiteSpace(guestName))
                    {
                        guestName = guest.Username ?? "Guest";
                    }
                }

                return new GuestRatingDetailsDTO
                {
                    ReservationId = reservationId,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    GuestName = guestName,
                    AccommodationName = accommodation?.Name ?? "Unknown Property",
                    Review = new GuestReviewDTO { ReservationId = reservationId }
                };
            }
            catch (Exception ex)
            {
                return new GuestRatingDetailsDTO
                {
                    ReservationId = reservationId,
                    StartDate = DateTime.Now.AddDays(-7),
                    EndDate = DateTime.Now,
                    GuestName = "Guest",
                    AccommodationName = "Property",
                    Review = new GuestReviewDTO { ReservationId = reservationId }
                };
            }
        }
    }
}