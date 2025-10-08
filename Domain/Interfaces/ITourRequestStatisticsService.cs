using System.Collections.Generic;
using BookingApp.Domain.Model;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourRequestStatisticsService
    {
        TourRequestStatistics GetStatisticsForTourist(int touristId, int? year = null);
        List<LanguageStatisticData> GetLanguageStatistics(int touristId, int? year = null);
        List<LocationStatisticData> GetLocationStatistics(int touristId, int? year = null);
    }
}
