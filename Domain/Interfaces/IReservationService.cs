using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationService
    {
        public ReservationDTO Create(ReservationDTO reservationDto);
        public List<DateTime> GetOccupiedDatesForAccommodation(int accommodationId);
        List<ReservationDTO> GetAll();
    }
}