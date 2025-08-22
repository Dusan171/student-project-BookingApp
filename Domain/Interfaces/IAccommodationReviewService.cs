using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationReviewService
    {
        // Metode koje se tiču recenzije smeštaja
        bool IsReviewPeriodExpired(Reservation reservation);
        void Create(Reservation reservation, int cleanliness, int ownerRating, string comment, string imagePaths);
        bool HasGuestRated(int reservationId);
        AccommodationReview GetByReservationId(int reservationId); // Ako vam treb
    }
}
