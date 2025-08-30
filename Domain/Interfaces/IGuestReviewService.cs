using BookingApp.Services.DTO;
using System.Collections.Generic;


namespace BookingApp.Domain.Interfaces
{
    public interface IGuestReviewService
    {
        List<GuestReviewDTO> GetAllReviews();
        void AddReview(GuestReviewDTO review);
        void DeleteReview(GuestReviewDTO review);
        GuestReviewDTO UpdateReview(GuestReviewDTO review);
        List<GuestReviewDTO> GetReviewsByReservation(ReservationDTO reservation);
        GuestReviewDTO GetReviewForReservation(int reservationId);

    }
}
