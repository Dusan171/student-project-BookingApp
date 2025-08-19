using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IGuestReviewRepository
    {
        List<GuestReview> GetAll();
        GuestReview Save(GuestReview review);
        void Delete(GuestReview review);
        GuestReview Update(GuestReview review);
        List<GuestReview> GetByReservationId(Reservation reservation);
    }
}
