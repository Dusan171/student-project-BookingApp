using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IGuestReviewService
    {
        List<GuestReviewDTO> GetAllReviews();
        GuestReviewDTO AddReview(GuestReviewDTO review);
        void DeleteReview(GuestReviewDTO review);
        GuestReviewDTO UpdateReview(GuestReviewDTO review);
        List<GuestReviewDTO> GetReviewsByReservation(ReservationDTO reservation);

    }
}
