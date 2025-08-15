using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IGuestReviewService
    {
        List<GuestReview> GetAllReviews();
        GuestReview AddReview(GuestReview review);
        void DeleteReview(GuestReview review);
        GuestReview UpdateReview(GuestReview review);
        List<GuestReview> GetReviewsByReservation(Reservation reservation);

    }
}
