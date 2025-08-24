using System;
using System.Collections.Generic;
using BookingApp.Services.DTO;
using BookingApp.Services.DTOs;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationService
    {
        public Reservation Create(ReservationDTO reservationDto);
        public List<DateTime> GetOccupiedDatesForAccommodation(int accommodationId);
    }
}
