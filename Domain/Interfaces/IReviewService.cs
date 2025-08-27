using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IReviewService
    {
        public bool IsReviewPeriodExpired(Reservation reservation);
        public void CreateOwnerReview(Reservation reservation, int cleanliness, int ownerRating, string comment, string imagePaths);
        bool HasGuestRated(int reservationId);
        //GuestReview GetReviewFromOwner(Reservation reservation);
    }
}
