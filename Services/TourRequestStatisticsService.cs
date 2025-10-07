// Services/TourRequestStatisticsService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;

namespace BookingApp.Services
{
    public class TourRequestStatisticsService : ITourRequestStatisticsService
    {
        private readonly ITourRequestRepository _tourRequestRepository;

        public TourRequestStatisticsService(ITourRequestRepository tourRequestRepository)
        {
            _tourRequestRepository = tourRequestRepository ?? throw new ArgumentNullException(nameof(tourRequestRepository));
        }

        public TourRequestStatistics GetStatisticsForTourist(int touristId, int? year = null)
        {
            var requests = GetFilteredRequests(touristId, year);

            var acceptedRequests = requests.Where(r => r.Status == TourRequestStatus.ACCEPTED).ToList();
            var notAcceptedRequests = requests.Where(r => r.Status != TourRequestStatus.ACCEPTED).ToList();

            var totalRequests = requests.Count;
            var acceptedCount = acceptedRequests.Count;
            var notAcceptedCount = notAcceptedRequests.Count;

            var acceptanceRate = totalRequests > 0
                ? (acceptedCount * 100.0 / totalRequests)
                : 0.0;

            var averagePeople = acceptedRequests.Any()
                ? acceptedRequests.Average(r => r.NumberOfPeople)
                : 0.0;

            return new TourRequestStatistics
            {
                TotalRequests = totalRequests,
                AcceptedCount = acceptedCount,
                NotAcceptedCount = notAcceptedCount,
                AcceptanceRate = acceptanceRate,
                AveragePeopleInAcceptedRequests = averagePeople
            };
        }

        public List<LanguageStatisticData> GetLanguageStatistics(int touristId, int? year = null)
        {
            var requests = GetFilteredRequests(touristId, year);

            return requests
                .GroupBy(r => r.Language)
                .Select(g => new LanguageStatisticData
                {
                    Language = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToList();
        }

        public List<LocationStatisticData> GetLocationStatistics(int touristId, int? year = null)
        {
            var requests = GetFilteredRequests(touristId, year);

            return requests
                .GroupBy(r => $"{r.City}, {r.Country}")
                .Select(g => new LocationStatisticData
                {
                    Location = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToList();
        }

        private List<TourRequest> GetFilteredRequests(int touristId, int? year)
        {
            var allRequests = _tourRequestRepository.GetByTouristId(touristId);

            if (year.HasValue)
            {
                return allRequests
                    .Where(r => r.CreatedAt.Year == year.Value)
                    .ToList();
            }

            return allRequests;
        }
    }
}