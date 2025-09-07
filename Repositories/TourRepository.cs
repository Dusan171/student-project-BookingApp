using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TourRepository : ITourRepository
    {
        private const string FilePath = "../../../Resources/Data/tours.csv";
        private readonly Serializer<Tour> _serializer;
        private List<Tour> _tours;

        public TourRepository()
        {
            _serializer = new Serializer<Tour>();
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
        }

        public void SaveAll() => _serializer.ToCSV(FilePath, _tours);

        private void Reload() => _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();

        public List<Tour> GetAll() => _tours.ToList();

        public Tour GetById(int id) => _tours.First(t => t.Id == id);

        public List<Tour> GetByLocation(string city, string country)
        {
            return _tours.Where(t => t.Location != null &&
                (!string.IsNullOrEmpty(t.Location.City) && t.Location.City.Equals(city, StringComparison.OrdinalIgnoreCase)) &&
                (!string.IsNullOrEmpty(t.Location.Country) && t.Location.Country.Equals(country, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        public List<Tour> GetByLanguage(string language)
            => _tours.Where(t => !string.IsNullOrEmpty(t.Language) && t.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<Tour> GetAvailableTours()
            => _tours.Where(t => t.MaxTourists - t.ReservedSpots > 0).ToList();

        public Tour Add(Tour tour)
        {
            tour.Id = GetNextId();
            _tours.Add(tour);
            SaveAll();
            return tour;
        }

        public Tour Update(Tour tour)
        {

            var index = _tours.FindIndex(t => t.Id == tour.Id);
            if (index >= 0)
            {
                _tours[index] = tour;
                SaveAll();
            }
            return tour;
        }
        public void Delete(int id)
        {
            var tour = _tours.FirstOrDefault(t => t.Id == id);
            if (tour != null)
            {
                _tours.Remove(tour);
                SaveAll();
            }
        }

        public int GetNextId() => _tours.Count == 0 ? 1 : _tours.Max(t => t.Id) + 1;

        public bool UpdateReservedSpots(int tourId, int newReservedSpots)
        {
            var tour = GetById(tourId);
            if (tour != null)
            {
                tour.ReservedSpots = newReservedSpots;
                SaveAll();
                return true;
            }
            return false;
        }

        public bool ReserveSpots(int tourId, int numberOfSpots)
        {
            var tour = GetById(tourId);
            if (tour != null && (tour.MaxTourists - tour.ReservedSpots) >= numberOfSpots)
            {
                tour.ReservedSpots += numberOfSpots;
                SaveAll();
                return true;
            }
            return false;
        }

        public List<Tour> GetByMaxTourists(int maxTourists)
        {
            return _tours.Where(t => t.MaxTourists <= maxTourists).ToList();
        }

        public Tour Save(Tour tour)
        {
            var existing = _tours.FirstOrDefault(t => t.Id == tour.Id);
            if (existing == null)
                return Add(tour);
            else
                return Update(tour);
        }

        public List<Tour> SearchTours(string location, string country, string language, int? maxPeople, double? duration)
        {
            var filtered = _tours.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(location))
                filtered = filtered.Where(t => t.Location?.City?.Contains(location, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrWhiteSpace(country))
                filtered = filtered.Where(t => t.Location?.Country?.Contains(country, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrWhiteSpace(language))
                filtered = filtered.Where(t => t.Language?.Equals(language, StringComparison.OrdinalIgnoreCase) == true);

            if (maxPeople.HasValue)
                filtered = filtered.Where(t => (t.MaxTourists - t.ReservedSpots) >= maxPeople.Value);

            if (duration.HasValue)
                filtered = filtered.Where(t => Math.Abs(t.DurationHours - duration.Value) < 0.1);

            return filtered.ToList();
        }
        public List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots)
        {
            var originalTour = GetById(originalTourId);
            if (originalTour?.Location == null) return new List<Tour>();

            return _tours.Where(t =>
                t.Id != originalTourId &&
                t.Location?.Id == originalTour.Location.Id &&
                (t.MaxTourists - t.ReservedSpots) >= requiredSpots
            ).ToList();
        }

        public List<KeyPoint> GetKeyPointsForTour(int tourId)
        {
            var tour = GetById(tourId);
            if (tour != null && tour.KeyPoints != null)
            {
                return tour.KeyPoints;
            }
            return new List<KeyPoint>();
        }
    }
}
