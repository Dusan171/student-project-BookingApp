using System;
using System.Collections.Generic;
using BookingApp.Domain;
using BookingApp.Services.DTOs;

namespace BookingApp.Domain.Interfaces
{
    public interface IRescheduleRequestService
    {
        //mora da radi sa modelom, jer mu treba citav objekat za dalja racunanja
        public List<DateTime> GetBlackoutDatesForReschedule(Reservation reservation);
        public void CreateRequest(CreateRescheduleRequestDTO requestDto);
    }
}
