using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationStatisticsService
    {
        
        List<YearlyStatisticDTO> GetYearlyStatistics(int accommodationId);
 
        List<MonthlyStatisticDTO> GetMonthlyStatistics(int accommodationId, int year);        

    }
}