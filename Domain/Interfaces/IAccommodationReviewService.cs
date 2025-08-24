using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationReviewService
    {
        // Metode koje se tiču recenzije smeštaja
        bool IsReviewPeriodExpired(ReservationDTO reservation);
        void Create(ReservationDTO reservation, int cleanliness, int ownerRating, string comment, string imagePaths);
        bool HasGuestRated(int reservationId);
        AccommodationReview GetByReservationId(int reservationId); // Ako vam treb
    }
}
