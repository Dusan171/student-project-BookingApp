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

        public List<Tour> GetAll()
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            return _tours.ToList();
        }

        public Tour GetById(int id)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            return _tours.FirstOrDefault(t => t.Id == id);
        }

        public List<Tour> GetByLocation(string city, string country)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            return _tours.Where(t => t.Location != null &&
                               t.Location.Id > 0).ToList();
        }

        public List<Tour> GetByLanguage(string language)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            return _tours.Where(t => !string.IsNullOrEmpty(t.Language) &&
                               t.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Tour> GetByMaxTourists(int maxTourists)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            return _tours.Where(t => t.MaxTourists >= maxTourists).ToList();
        }

        public List<Tour> GetAvailableTours()
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            return _tours.Where(t => t.ReservedSpots < t.MaxTourists).ToList();
        }

        public Tour Add(Tour tour)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            tour.Id = GetNextId();
            _tours.Add(tour);
            SaveAll();
            return tour;
        }

        public Tour Save(Tour tour)
        {
            if (tour.Id == 0)
            {
                // New tour - add it
                return Add(tour);
            }
            else
            {
                // Existing tour - update it
                return Update(tour);
            }
        }

        public Tour Update(Tour tour)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            var existing = _tours.FirstOrDefault(t => t.Id == tour.Id);
            if (existing != null)
            {
                int index = _tours.IndexOf(existing);
                _tours[index] = tour;
                SaveAll();
            }
            return tour;
        }

        public void Delete(int id)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            var tour = _tours.FirstOrDefault(t => t.Id == id);
            if (tour != null)
            {
                _tours.Remove(tour);
                SaveAll();
            }
        }

        public void SaveAll()
        {
            _serializer.ToCSV(FilePath, _tours);
        }

        public int GetNextId()
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            return _tours.Count == 0 ? 1 : _tours.Max(t => t.Id) + 1;
        }

        public bool UpdateReservedSpots(int tourId, int newReservedSpots)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();
            var tour = _tours.FirstOrDefault(t => t.Id == tourId);
            if (tour != null)
            {
                tour.ReservedSpots = newReservedSpots;
                SaveAll();
                return true;
            }
            return false;
        }

        // Add the missing SearchTours method with correct signature
        public List<Tour> SearchTours(string location, string country, string language, int maxPeople, double duration)
        {
            _tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();

            var filteredTours = _tours.AsEnumerable(); // Use AsEnumerable() instead of AsQueryable()

            if (!string.IsNullOrWhiteSpace(location))
            {
                filteredTours = filteredTours.Where(t => t.Location != null &&
                    ((!string.IsNullOrEmpty(t.Location.City) && t.Location.City.Contains(location, StringComparison.OrdinalIgnoreCase)) ||
                     (!string.IsNullOrEmpty(t.Location.Country) && t.Location.Country.Contains(location, StringComparison.OrdinalIgnoreCase))));
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                filteredTours = filteredTours.Where(t => t.Location != null &&
                    !string.IsNullOrEmpty(t.Location.Country) &&
                    t.Location.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                filteredTours = filteredTours.Where(t =>
                    !string.IsNullOrEmpty(t.Language) &&
                    t.Language.Equals(language, StringComparison.OrdinalIgnoreCase));
            }

            if (maxPeople > 0)
            {
                filteredTours = filteredTours.Where(t => (t.MaxTourists - t.ReservedSpots) >= maxPeople);
            }

            if (duration > 0)
            {
                filteredTours = filteredTours.Where(t => Math.Abs(t.DurationHours - duration) < 0.1);
            }

            return filteredTours.ToList();
        }

        // Overload to handle nullable parameters from TourSearch.xaml.cs
        public List<Tour> SearchTours(string location, string country, string language, int? maxPeople, double? duration)
        {
            return SearchTours(location, country, language, maxPeople ?? 0, duration ?? 0.0);
        }

        public bool ReserveSpots(int tourId, int numberOfSpots)
        {
            var tour = GetById(tourId);
            if (tour != null)
            {
                int availableSpots = tour.MaxTourists - tour.ReservedSpots;
                if (availableSpots >= numberOfSpots)
                {
                    int newReservedSpots = tour.ReservedSpots + numberOfSpots;
                    return UpdateReservedSpots(tourId, newReservedSpots);
                }
            }
            return false;
        }

        public List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots)
        {
            var tours = GetAll();
            var originalTour = GetById(originalTourId);

            if (originalTour?.Location == null)
                return new List<Tour>();

            return tours.Where(t =>
                t.Id != originalTourId &&
                t.Location != null &&
                t.Location.Id == originalTour.Location.Id && // Compare by ID since Location details might not be loaded
                (t.MaxTourists - t.ReservedSpots) >= requiredSpots
            ).ToList();
        }



    }
}