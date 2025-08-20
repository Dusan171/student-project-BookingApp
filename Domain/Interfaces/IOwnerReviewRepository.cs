using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain;

namespace BookingApp.Domain.Interfaces
{
    public interface IOwnerReviewRepository
    {
        List<OwnerReview> GetAll();
        List<OwnerReview> GetByReservationId(int reservationId);
        bool HasGuestRated(int reservationId);
        OwnerReview Save(OwnerReview review);
        int NextId();
    }
}
