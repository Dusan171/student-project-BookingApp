using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BookingApp.Services
{
    public class AccommodationStatisticsService : IAccommodationStatisticsService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRescheduleRequestRepository _rescheduleRequestRepository;

        public AccommodationStatisticsService(
            IReservationRepository reservationRepository,
            IRescheduleRequestRepository rescheduleRequestRepository)
        {
            _reservationRepository = reservationRepository;
            _rescheduleRequestRepository = rescheduleRequestRepository;
        }

        public List<YearlyStatisticDTO> GetYearlyStatistics(int accommodationId)
        {
            var reservations = _reservationRepository.GetByAccommodationId(accommodationId);
            if (!reservations.Any())
                return new List<YearlyStatisticDTO>();

            var years = reservations
                .SelectMany(r => new[] { r.StartDate.Year, r.EndDate.Year }).Distinct().OrderBy(y => y).ToList();
            var yearlyStats = new List<YearlyStatisticDTO>();
            foreach (var year in years)
            {
                var yearReservations = reservations.Where(r => r.StartDate.Year == year || r.EndDate.Year == year).ToList();
                var reschedules = GetReschedulesForReservations(yearReservations);
                var stats = new YearlyStatisticDTO{Year = year,ReservationCount = yearReservations.Count,CancellationCount = yearReservations.Count(r => r.Status == ReservationStatus.Cancelled),RescheduleCount = reschedules.Count,
                OccupancyRate = CalculateYearlyOccupancyRate(yearReservations, year) }; yearlyStats.Add(stats);
            }
            return yearlyStats;
        }
        private bool DoesReservationOverlapMonth(Reservation r, int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return (r.StartDate.Year == year && r.StartDate.Month == month) ||
                   (r.EndDate.Year == year && r.EndDate.Month == month) ||
                   (r.StartDate <= monthStart && r.EndDate >= monthEnd);
            
        }
        public List<MonthlyStatisticDTO> GetMonthlyStatistics(int accommodationId, int year)
        {
            var reservations = _reservationRepository.GetByAccommodationId(accommodationId);
            var monthlyStats = new List<MonthlyStatisticDTO>();
            for (int month = 1; month <= 12; month++)
            {
                var monthReservations = reservations
            .Where(r => DoesReservationOverlapMonth(r, year, month))
            .ToList();
                var reschedules = GetReschedulesForReservations(monthReservations);

                var stats = new MonthlyStatisticDTO
                {
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Year = year,
                    ReservationCount = monthReservations.Count,
                    CancellationCount = monthReservations.Count(r => r.Status == ReservationStatus.Cancelled),
                    RescheduleCount = reschedules.Count,
                    OccupancyRate = CalculateMonthlyOccupancyRate(monthReservations, year, month)
                };

                monthlyStats.Add(stats);
            }

            return monthlyStats;
        }
        private List<RescheduleRequest> GetReschedulesForReservations(List<Reservation> reservations)
        {
            var reservationIds = reservations.Select(r => r.Id).ToList();
            return _rescheduleRequestRepository.GetAll()
                .Where(rr => reservationIds.Contains(rr.ReservationId) &&
                            rr.Status == RequestStatus.Approved)
                .ToList();
        }

        private double CalculateYearlyOccupancyRate(List<Reservation> reservations, int year)
        {
            var occupiedDates = new HashSet<DateTime>();
            var yearStart = new DateTime(year, 1, 1);
            var yearEnd = new DateTime(year, 12, 31);

            foreach (var reservation in reservations.Where(r => r.Status != ReservationStatus.Cancelled))
            {
                var startDate = reservation.StartDate > yearStart ? reservation.StartDate : yearStart;
                var endDate = reservation.EndDate < yearEnd ? reservation.EndDate : yearEnd;

                for (var date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
                {
                    occupiedDates.Add(date);
                }
            }

            int totalDaysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            return Math.Round((double)occupiedDates.Count / totalDaysInYear * 100, 2);
        }

        private double CalculateMonthlyOccupancyRate(List<Reservation> reservations, int year, int month)
        {
            var occupiedDates = new HashSet<DateTime>();
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            foreach (var reservation in reservations.Where(r => r.Status != ReservationStatus.Cancelled))
            {
                var startDate = reservation.StartDate > monthStart ? reservation.StartDate : monthStart;
                var endDate = reservation.EndDate < monthEnd ? reservation.EndDate : monthEnd;

                for (var date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
                {
                    occupiedDates.Add(date);
                }
            }

            int totalDaysInMonth = DateTime.DaysInMonth(year, month);
            return Math.Round((double)occupiedDates.Count / totalDaysInMonth * 100, 2);
        }
    }




























    public class AccommodationSummaryService: IAccommodationSummaryService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccommodationStatisticsService _statisticsService;
        public AccommodationSummaryService(
            IReservationRepository reservationRepository,
            IAccommodationStatisticsService statisticsService)
        {
            _reservationRepository = reservationRepository;
            _statisticsService = statisticsService;
        }

        public AccommodationStatisticsSummaryDTO GetStatisticsSummary(int accommodationId)
        {
            var yearlyStats = _statisticsService.GetYearlyStatistics(accommodationId);

            if (!yearlyStats.Any()) { return new AccommodationStatisticsSummaryDTO(); }

            var totalReservations = yearlyStats.Sum(s => s.ReservationCount);
            var totalCancellations = yearlyStats.Sum(s => s.CancellationCount);
            var totalReschedules = yearlyStats.Sum(s => s.RescheduleCount);

            var bestYear = GetBestPerformingYear(accommodationId);
            string bestPeriodDescription = "No data available";
            double bestPeriodOccupancy = 0;

            if (bestYear != null)
            {
                var bestMonth = GetBestPerformingMonth(accommodationId, bestYear.Year);
                if (bestMonth != null && bestMonth.OccupancyRate > bestYear.OccupancyRate)
                {
                    bestPeriodDescription = $"{bestMonth.MonthName} {bestMonth.Year}";
                    bestPeriodOccupancy = bestMonth.OccupancyRate;
                }
                else
                {
                    bestPeriodDescription = bestYear.Year.ToString();
                    bestPeriodOccupancy = bestYear.OccupancyRate;
                }
            }
            return new AccommodationStatisticsSummaryDTO { TotalReservations = totalReservations, TotalCancellations = totalCancellations, TotalReschedules = totalReschedules, BestYear = bestYear?.Year ?? 0, BestPeriodDescription = bestPeriodDescription, BestPeriodOccupancy = bestPeriodOccupancy };
        }

        public List<int> GetAvailableYears(int accommodationId)
        {
            var reservations = _reservationRepository.GetByAccommodationId(accommodationId);

            if (!reservations.Any())
                return new List<int>();

            return reservations
                .SelectMany(r => new[] { r.StartDate.Year, r.EndDate.Year })
                .Distinct()
                .OrderBy(y => y)
                .ToList();
        }

        public YearlyStatisticDTO GetBestPerformingYear(int accommodationId)
        {
            var yearlyStats = _statisticsService.GetYearlyStatistics(accommodationId);
            return yearlyStats.OrderByDescending(s => s.OccupancyRate).FirstOrDefault();
        }

        public MonthlyStatisticDTO GetBestPerformingMonth(int accommodationId, int year)
        {
            var monthlyStats = _statisticsService.GetMonthlyStatistics(accommodationId, year);
            return monthlyStats
                .Where(s => s.ReservationCount > 0)
                .OrderByDescending(s => s.OccupancyRate)
                .FirstOrDefault();
        }
    }


    }