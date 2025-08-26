using System;
using System.Collections.Generic;
using BookingApp.Domain;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface IRescheduleRequestService
    {
        public List<DateTime> GetBlackoutDatesForReschedule(ReservationDTO reservation);
        public void CreateRequest(RescheduleRequestDTO requestDto);
        List<RescheduleRequestDTO> GetAll();
        RescheduleRequestDTO GetById(int id);
        void Update(RescheduleRequestDTO request);
    }
}
