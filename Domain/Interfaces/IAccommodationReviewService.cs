using System;
using System.Collections.Generic;
using BookingApp.Domain.Model;
using BookingApp.DTO;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationReviewService
    {
        bool IsReviewPeriodExpired(ReservationDTO reservation);
        void Create(ReservationDTO reservation, int cleanliness, int ownerRating, string comment, string imagePaths);
        bool HasGuestRated(int reservationId);
        AccommodationReview GetByReservationId(int reservationId); 
        List<AccommodationReviewDTO> GetAll();
    }
}