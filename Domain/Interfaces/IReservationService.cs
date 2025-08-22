using System;
using System.Collections.Generic;
using BookingApp.Services.DTOs;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationService
    {
        public Reservation Create(CreateReservationDTO reservationDto);
        public List<DateTime> GetOccupiedDatesForAccommodation(int accommodationId);
    }
}
