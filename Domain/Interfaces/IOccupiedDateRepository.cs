using BookingApp.Domain;
using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces 
{
    public interface IOccupiedDateRepository
    {
        List<OccupiedDate> GetAll();
        List<OccupiedDate> GetByAccommodationId(int accommodationId);
        void Save(List<OccupiedDate> newDates);
        int NextId();
    }
}