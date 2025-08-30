using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IStartTourTimeService
    {
        List<StartTourTimeDTO> GetAllStartTourTimes();
        StartTourTimeDTO? GetStartTourTimeById(int id);
        StartTourTimeDTO SaveStartTourTime(StartTourTimeDTO startTourTimeDTO);
        StartTourTimeDTO UpdateStartTourTime(StartTourTimeDTO startTourTimeDTO);
        void DeleteStartTourTime(StartTourTimeDTO startTourTimeDTO);
        List<StartTourTimeDTO> GetStartTourTimesByTourId(int tourId);
        bool IsTimeAvailable(StartTourTimeDTO startTourTimeDTO);
        List<StartTourTimeDTO> GetAvailableTimesForDate(DateTime date);
        bool ValidateStartTourTime(StartTourTimeDTO startTourTimeDTO);
        List<StartTourTimeDTO> GetFutureStartTimes();
        List<StartTourTimeDTO> GetStartTimesInDateRange(DateTime startDate, DateTime endDate);
    }
}