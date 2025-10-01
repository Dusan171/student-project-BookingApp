using System;
using System.Collections.Generic;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationCreationService
    {
        ReservationAttemptResult AttemptReservation(ReservationDTO reservationDto);
        List<DateTime> GetOccupiedDatesForAccommodation(int accommodationId);
    }
}