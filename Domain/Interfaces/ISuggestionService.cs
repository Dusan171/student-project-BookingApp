using System;
using System.Collections.Generic;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface ISuggestionService
    {
        List<DateRange> FindAvailableDateRanges(int accommodationId, int duration, DateTime preferredStartDate);
    }
}