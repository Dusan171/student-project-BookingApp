using System;
using System.Collections.Generic;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface IRescheduleRequestService
    {
        public List<DateTime> GetBlackoutDatesForReschedule(ReservationDTO reservationDto);
        public void CreateRequest(RescheduleRequestDTO requestDto);
    }
}
