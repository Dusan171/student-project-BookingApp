using BookingApp.Domain;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TourRepository
    {
        private const string FilePath = "../../../Resources/Data/tours.csv";
        private readonly Serializer<Tour> _serializer;
        private readonly LocationRepository _locationRepository;
        private readonly UserRepository _userRepository;
        private List<Tour> _tours;

        public TourRepository()
        {
            _serializer = new Serializer<Tour>();
            _locationRepository = new LocationRepository();
            _userRepository = new UserRepository();
            _tours = LoadToursWithLocationsAndGuides();
        }

        private List<Tour> LoadToursWithLocationsAndGuides()
        {
            try
            {
                var tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();

                foreach (var tour in tours)
                {
                    // ➡️ Učitaj lokaciju
                    if (tour.Location != null && tour.Location.Id > 0)
                    {
                        var fullLocation = _locationRepository.GetById(tour.Location.Id);
                        if (fullLocation != null)
                        {
                            tour.Location = fullLocation;
                        }
                    }

                    // ➡️ Učitaj vodiča (User)
                    if (tour.Guide != null && tour.Guide.Id > 0)
                    {
                        var fullGuide = _userRepository.GetById(tour.Guide.Id);
                        if (fullGuide != null)
                        {
                            tour.Guide = fullGuide;
                        }
                    }
                }

                return tours;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Greška pri učitavanju tura: {ex.Message}");
                return new List<Tour>();
            }
        }

        public List<Tour> GetAll()
        {
            _tours = LoadToursWithLocationsAndGuides();
            return _tours;
        }

        public List<Tour> GetAvailableTours()
        {
            _tours = LoadToursWithLocationsAndGuides();
            return _tours.Where(t => t.ReservedSpots < t.MaxTourists).ToList();
        }

        public List<Tour> SearchTours(string city = null, string country = null,
            string language = null, int? maxPeople = null, double? duration = null)
        {
            _tours = LoadToursWithLocationsAndGuides();

            if (_tours == null || _tours.Count == 0)
                return new List<Tour>();

            var query = _tours.AsQueryable();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(t => t.Location != null &&
                    !string.IsNullOrEmpty(t.Location.City) &&
                    t.Location.City.Contains(city, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(t => t.Location != null &&
                    !string.IsNullOrEmpty(t.Location.Country) &&
                    t.Location.Country.Contains(country, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                query = query.Where(t => !string.IsNullOrEmpty(t.Language) &&
                    t.Language.Equals(language, StringComparison.OrdinalIgnoreCase));
            }

            if (maxPeople.HasValue)
            {
                query = query.Where(t => t.AvailableSpots >= maxPeople.Value);
            }

            if (duration.HasValue)
            {
                query = query.Where(t => Math.Abs(t.DurationHours - duration.Value) < 0.1);
            }

            try
            {
                return query.ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Greška u pretrazi: {ex.Message}");
                return new List<Tour>();
            }
        }

        public bool ReserveSpots(int tourId, int numberOfSpots)
        {
            _tours = LoadToursWithLocationsAndGuides();
            var tour = _tours?.FirstOrDefault(t => t.Id == tourId);

            if (tour != null && tour.AvailableSpots >= numberOfSpots)
            {
                tour.ReservedSpots += numberOfSpots;
                _serializer.ToCSV(FilePath, _tours);
                return true;
            }
            return false;
        }

        public List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots)
        {
            _tours = LoadToursWithLocationsAndGuides();
            if (_tours == null || _tours.Count == 0)
                return new List<Tour>();

            var originalTour = _tours.FirstOrDefault(t => t.Id == originalTourId);
            if (originalTour?.Location == null)
                return new List<Tour>();

            return _tours.Where(t =>
                t.Id != originalTourId &&
                t.Location != null &&
                t.Location.City != null && t.Location.Country != null &&
                originalTour.Location.City != null && originalTour.Location.Country != null &&
                t.Location.City.Trim().Equals(originalTour.Location.City.Trim(), StringComparison.OrdinalIgnoreCase) &&
                t.Location.Country.Trim().Equals(originalTour.Location.Country.Trim(), StringComparison.OrdinalIgnoreCase) &&
                t.AvailableSpots >= requiredSpots
            ).ToList();
        }

        public Tour GetById(int id)
        {
            _tours = LoadToursWithLocationsAndGuides();
            return _tours?.FirstOrDefault(t => t.Id == id);
        }

        public Tour Save(Tour tour)
        {
            _tours = LoadToursWithLocationsAndGuides();
            tour.Id = NextId();
            _tours.Add(tour);
            _serializer.ToCSV(FilePath, _tours);
            return tour;
        }

        public int NextId()
        {
            _tours = LoadToursWithLocationsAndGuides();
            if (_tours == null || _tours.Count < 1)
            {
                return 1;
            }
            return _tours.Max(c => c.Id) + 1;
        }

        public void Delete(Tour tour)
        {
            _tours = LoadToursWithLocationsAndGuides();
            if (_tours == null) return;

            Tour found = _tours.Find(c => c.Id == tour.Id);
            if (found != null)
            {
                _tours.Remove(found);
                _serializer.ToCSV(FilePath, _tours);
            }
        }

        public Tour Update(Tour tour)
        {
            _tours = LoadToursWithLocationsAndGuides();
            if (_tours == null) return tour;

            Tour current = _tours.Find(c => c.Id == tour.Id);
            if (current != null)
            {
                int index = _tours.IndexOf(current);
                _tours.Remove(current);
                _tours.Insert(index, tour);
                _serializer.ToCSV(FilePath, _tours);
            }
            return tour;
        }
    }
}
