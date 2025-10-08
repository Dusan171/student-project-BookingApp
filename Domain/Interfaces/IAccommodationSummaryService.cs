using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces.ServiceInterfaces
{
    public interface IAccommodationSummaryService
    {
        AccommodationStatisticsSummaryDTO GetStatisticsSummary(int accommodationId);
        YearlyStatisticDTO GetBestPerformingYear(int accommodationId);
        MonthlyStatisticDTO GetBestPerformingMonth(int accommodationId, int year);
        List<int> GetAvailableYears(int accommodationId);
    }
}