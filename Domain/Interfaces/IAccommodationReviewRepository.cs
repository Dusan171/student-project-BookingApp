using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationReviewRepository
    {
        List<AccommodationReview> GetAll();
        List<AccommodationReview> GetByReservationId(int reservationId);
        bool HasGuestRated(int reservationId);
        AccommodationReview Save(AccommodationReview review);
        int NextId();

    }
}
