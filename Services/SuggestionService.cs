using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class SuggestionService : ISuggestionService
    {
        private readonly IOccupiedDateRepository _occupiedDateRepository;

        public SuggestionService(IOccupiedDateRepository occupiedDateRepository)
        {
            _occupiedDateRepository = occupiedDateRepository;
        }

        public List<DateRange> FindAvailableDateRanges(int accommodationId, int duration, DateTime preferredStartDate)
        {
            var occupiedDates = _occupiedDateRepository.GetByAccommodationId(accommodationId)
                                                      .Select(o => o.Date.Date)
                                                      .ToHashSet();

            var suggestions = new List<DateRange>();

            suggestions.AddRange(SearchInDirection(occupiedDates, duration, preferredStartDate, -1, 3));

            suggestions.AddRange(SearchInDirection(occupiedDates, duration, preferredStartDate, 1, 6 - suggestions.Count));

            return suggestions.OrderBy(r => r.StartDate).ToList();
        }

        private IEnumerable<DateRange> SearchInDirection(HashSet<DateTime> occupiedDates, int duration, DateTime startDate, int direction, int maxSuggestions)
        {
            var foundSuggestions = new List<DateRange>();
            for (int i = 1; i <= 30; i++)
            {
                if (foundSuggestions.Count >= maxSuggestions) break;

                var checkStartDate = startDate.AddDays(direction * i);
                if (IsDateRangeAvailable(occupiedDates, checkStartDate, duration))
                {
                    foundSuggestions.Add(new DateRange { StartDate = checkStartDate, EndDate = checkStartDate.AddDays(duration) });
                }
            }
            return foundSuggestions;
        }

        private bool IsDateRangeAvailable(HashSet<DateTime> occupiedDates, DateTime startDate, int duration)
        {
            for (int i = 0; i < duration; i++)
            {
                if (occupiedDates.Contains(startDate.AddDays(i)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}