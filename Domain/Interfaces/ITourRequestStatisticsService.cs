// Domain/Interfaces/ITourRequestStatisticsService.cs
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourRequestStatisticsService
    {
        TourRequestStatistics GetStatisticsForTourist(int touristId, int? year = null);
        List<LanguageStatisticData> GetLanguageStatistics(int touristId, int? year = null);
        List<LocationStatisticData> GetLocationStatistics(int touristId, int? year = null);
    }

    public class TourRequestStatistics
    {
        public int TotalRequests { get; set; }
        public int AcceptedCount { get; set; }
        public int NotAcceptedCount { get; set; }
        public double AcceptanceRate { get; set; }
        public double AveragePeopleInAcceptedRequests { get; set; }
    }

    public class LanguageStatisticData
    {
        public string Language { get; set; }
        public int Count { get; set; }
    }

    public class LocationStatisticData
    {
        public string Location { get; set; }
        public int Count { get; set; }
    }
}