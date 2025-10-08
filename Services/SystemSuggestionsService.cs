using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services
{
    public class SystemSuggestionsService : ISystemSuggestionsService
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ILocationRepository _locationRepository;

        private const int ANALYSIS_PERIOD_DAYS = 30;

        public SystemSuggestionsService(
            IAccommodationRepository accommodationRepository,
            IReservationRepository reservationRepository,
            ILocationRepository locationRepository)
        {
            _accommodationRepository = accommodationRepository;
            _reservationRepository = reservationRepository;
            _locationRepository = locationRepository;
        }
        public List<HighDemandLocationDTO> GetHighDemandLocations(int ownerId)
        {
            var ownerAccommodations = _accommodationRepository.GetByOwnerId(ownerId).ToList();
            if (!ownerAccommodations.Any())
                return new List<HighDemandLocationDTO>();
            var cutoffDate = DateTime.Now.Date.AddDays(-ANALYSIS_PERIOD_DAYS);
            var locationStats = AggregateLocationStatistics(ownerAccommodations, cutoffDate);
            return SelectTopLocations(locationStats);
        }
        private List<Reservation> GetRelevantReservations(int accommodationId, DateTime cutoffDate)
        {
            return _reservationRepository.GetByAccommodationId(accommodationId)
                .Where(r => r.Status != ReservationStatus.Cancelled &&
                            r.StartDate >= cutoffDate)
                .ToList();
        }

        private Dictionary<int, LocationStatistics> AggregateLocationStatistics(List<Accommodation> accommodations, DateTime cutoffDate)
        {
            var locationStats = new Dictionary<int, LocationStatistics>();

            foreach (var accommodation in accommodations)
            {
                var reservations = GetRelevantReservations(accommodation.Id, cutoffDate);

                if (!reservations.Any())
                    continue;

                int locationId = accommodation.GeoLocation.Id;

                if (!locationStats.ContainsKey(locationId))
                {
                    locationStats[locationId] = new LocationStatistics
                    {
                        LocationId = locationId,
                        ReservationCount = 0,
                        TotalOccupancyRate = 0,
                        AccommodationCount = 0
                    };
                }

                var stats = locationStats[locationId];
                stats.ReservationCount += reservations.Count;
                var avgOccupancy = CalculateAverageOccupancy(reservations, (int)accommodation.MaxGuests);
                stats.TotalOccupancyRate += avgOccupancy;
                stats.AccommodationCount++;
            }

            return locationStats;
        }

        // POMOĆNA METODA 1: Kreira listu statistika za sve smeštaje
        private List<AccommodationStatistic> AggregateAccommodationStatistics(List<Accommodation> accommodations, DateTime cutoffDate)
        {
            var accommodationStats = new List<AccommodationStatistic>();

            foreach (var accommodation in accommodations)
            {
                // Koristimo DRY princip - pozivamo već postojeću metodu
                var reservations = GetRelevantReservations(accommodation.Id, cutoffDate);

                var reservationCount = reservations.Count;

                // MNOC (poređenje) je izolovano ovde
                var avgOccupancy = reservations.Any()
                    ? CalculateAverageOccupancy(reservations, (int)accommodation.MaxGuests)
                    : 0;

                accommodationStats.Add(new AccommodationStatistic
                {
                    Accommodation = accommodation,
                    ReservationCount = reservationCount,
                    OccupancyRate = avgOccupancy,
                    // Kalkulacija bodova
                    Score = reservationCount * 0.5 + avgOccupancy * 0.5
                });
            }

            return accommodationStats;
        }
        private List<LowDemandAccommodationDTO> SelectBottomAccommodations(List<AccommodationStatistic> accommodationStats)
        {
            var sortedAccommodations = accommodationStats
                .OrderBy(a => a.Score)
                .ToList();
            int bottomCount = Math.Max(3, (int)Math.Ceiling(sortedAccommodations.Count * 0.3));
            var lowDemandAccommodations = new List<LowDemandAccommodationDTO>();

            foreach (var stat in sortedAccommodations.Take(bottomCount))
            {
                var location = _locationRepository.GetById(stat.Accommodation.GeoLocation.Id);

                lowDemandAccommodations.Add(new LowDemandAccommodationDTO
                {
                    AccommodationId = stat.Accommodation.Id,
                    AccommodationName = stat.Accommodation.Name,
                    City = location?.City ?? "Unknown",
                    Country = location?.Country ?? "Unknown",
                    ReservationCount = stat.ReservationCount,
                    OccupancyRate = Math.Round(stat.OccupancyRate, 1)
                });
            }
            return lowDemandAccommodations;
        }

        private List<HighDemandLocationDTO> SelectTopLocations(Dictionary<int, LocationStatistics> locationStats)
        {
            var sortedLocations = locationStats.Values
                .Select(s => new
                {
                    Stats = s,
                    AvgOccupancy = s.TotalOccupancyRate / s.AccommodationCount,
                    Score = s.ReservationCount * 0.5 + (s.TotalOccupancyRate / s.AccommodationCount) * 0.5
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            int topCount = Math.Max(3, (int)Math.Ceiling(sortedLocations.Count * 0.3));
            var highDemandLocations = new List<HighDemandLocationDTO>();

            foreach (var item in sortedLocations.Take(topCount))
            {
                var location = _locationRepository.GetById(item.Stats.LocationId);
                if (location == null) continue;

                highDemandLocations.Add(new HighDemandLocationDTO
                {
                    City = location.City,
                    Country = location.Country,
                    ReservationCount = item.Stats.ReservationCount,
                    OccupancyRate = Math.Round(item.AvgOccupancy, 1)
                });
            }

            return highDemandLocations;
        }
        public List<LowDemandAccommodationDTO> GetLowDemandAccommodations(int ownerId)
        {
            var ownerAccommodations = _accommodationRepository.GetByOwnerId(ownerId).ToList();
            if (!ownerAccommodations.Any())
                return new List<LowDemandAccommodationDTO>();
            var cutoffDate = DateTime.Now.Date.AddDays(-ANALYSIS_PERIOD_DAYS);
            var accommodationStats = AggregateAccommodationStatistics(ownerAccommodations, cutoffDate);
            return SelectBottomAccommodations(accommodationStats);
        }

        private double CalculateAverageOccupancy(List<Reservation> reservations, int maxGuests)
        {
            if (!reservations.Any() || maxGuests == 0)
                return 0;

            var totalOccupancy = reservations.Sum(r => (r.GuestsNumber / (double)maxGuests) * 100);
            return totalOccupancy / reservations.Count;
        }

        private class LocationStatistics
        {
            public int LocationId { get; set; }
            public int ReservationCount { get; set; }
            public double TotalOccupancyRate { get; set; }
            public int AccommodationCount { get; set; }
        }

        private class AccommodationStatistic
        {
            public Accommodation Accommodation { get; set; }
            public int ReservationCount { get; set; }
            public double OccupancyRate { get; set; }
            public double Score { get; set; }
        }
    }
}