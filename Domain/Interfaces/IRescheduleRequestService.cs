using System;
using System.Collections.Generic;
using BookingApp.Domain;
using BookingApp.Services.DTO;
using BookingApp.Services.DTOs;

namespace BookingApp.Domain.Interfaces
{
    public interface IRescheduleRequestService
    {
        public List<DateTime> GetBlackoutDatesForReschedule(Reservation reservation);
        public void CreateRequest(RescheduleRequestDTO requestDto);
        List<RescheduleRequestDTO> GetAll();
        RescheduleRequestDTO GetById(int id);
        void Update(RescheduleRequestDTO request);
    }
}
