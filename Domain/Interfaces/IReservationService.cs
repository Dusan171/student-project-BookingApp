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
        ReservationDTO GetById(int id);
        void Update(ReservationDTO reservationDto);
        bool IsAccommodationAvailable(int accommodationId, DateTime newStartDate, DateTime newEndDate);
    }
}