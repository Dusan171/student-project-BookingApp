using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services.Search
{
    public class TourSearchProcessor
    {
        public List<Tour> ProcessSearchCriteria(List<Tour> tours, SearchCriteriaDTO criteria, Func<int, int> getAvailableSpots)
        {
            if (criteria == null) return new List<Tour>();

            var query = tours.AsQueryable();

            query = ApplyLocationFilters(query, criteria);
            query = ApplyLanguageFilter(query, criteria);
            query = ApplyDurationFilter(query, criteria);

            try
            {
                var results = query.ToList();
                
                // Apply capacity filter separately since it requires getAvailableSpots function
                if (criteria.MaxPeople.HasValue)
                {
                    results = results.Where(t => getAvailableSpots(t.Id) >= criteria.MaxPeople.Value).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search processing error: {ex.Message}");
                return new List<Tour>();
            }
        }

        public List<Tour> FindAlternativeTours(List<Tour> tours, int originalTourId, int requiredSpots, Func<int, int> getAvailableSpots)
        {
            var originalTour = tours.FirstOrDefault(t => t.Id == originalTourId);
            if (originalTour?.Location == null) return new List<Tour>();

            var alternatives = tours.Where(t =>
                t.Id != originalTourId &&
                t.Location != null &&
                t.Location.Id == originalTour.Location.Id &&
                getAvailableSpots(t.Id) >= requiredSpots
            ).ToList();

            return alternatives;
        }

        private IQueryable<Tour> ApplyLocationFilters(IQueryable<Tour> query, SearchCriteriaDTO criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.City))
            {
                query = query.Where(t => t.Location != null &&
                    !string.IsNullOrEmpty(t.Location.City) &&
                    t.Location.City.Contains(criteria.City, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Country))
            {
                query = query.Where(t => t.Location != null &&
                    !string.IsNullOrEmpty(t.Location.Country) &&
                    t.Location.Country.Contains(criteria.Country, StringComparison.OrdinalIgnoreCase));
            }

            return query;
        }

        private IQueryable<Tour> ApplyLanguageFilter(IQueryable<Tour> query, SearchCriteriaDTO criteria)
        {
            if (!string.IsNullOrWhiteSpace(criteria.Language))
            {
                query = query.Where(t => !string.IsNullOrEmpty(t.Language) &&
                    t.Language.Equals(criteria.Language, StringComparison.OrdinalIgnoreCase));
            }
            return query;
        }

        private IQueryable<Tour> ApplyDurationFilter(IQueryable<Tour> query, SearchCriteriaDTO criteria)
        {
            if (criteria.Duration.HasValue)
            {
                query = query.Where(t => Math.Abs(t.DurationHours - criteria.Duration.Value) < 0.1);
            }
            return query;
        }
    }
}