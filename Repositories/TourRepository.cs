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
        private List<Tour> _tours;

        public TourRepository()
        {
            _serializer = new Serializer<Tour>();
            _locationRepository = new LocationRepository();
            _tours = LoadToursWithLocations();
        }

        private List<Tour> LoadToursWithLocations()
        {
            try
            {
                var tours = _serializer.FromCSV(FilePath) ?? new List<Tour>();

                // Popuni Location objekte sa podacima iz LocationRepository
                foreach (var tour in tours)
                {
                    if (tour.Location != null && tour.Location.Id > 0)
                    {
                        var fullLocation = _locationRepository.GetById(tour.Location.Id);
                        if (fullLocation != null)
                        {
                            tour.Location = fullLocation;
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
            _tours = LoadToursWithLocations();
            return _tours;
        }

        public List<Tour> GetAvailableTours()
        {
            _tours = LoadToursWithLocations();
            return _tours.Where(t => t.ReservedSpots < t.MaxTourists).ToList();
        }

        public List<Tour> SearchTours(string city = null, string country = null,
            string language = null, int? maxPeople = null, double? duration = null)
        {
            _tours = LoadToursWithLocations();

            if (_tours == null || _tours.Count == 0)
            {
                return new List<Tour>();
            }

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
            _tours = LoadToursWithLocations();
            var tour = _tours?.FirstOrDefault(t => t.Id == tourId);

            if (tour != null && tour.AvailableSpots >= numberOfSpots)
            {
                tour.ReservedSpots += numberOfSpots;
                _serializer.ToCSV(FilePath, _tours);
                return true;
            }
            return false;
        }

        // KLJUČNA METODA ZA ALTERNATIVNE TURE
        public List<Tour> GetAlternativeTours(int originalTourId, int requiredSpots)
        {
            _tours = LoadToursWithLocations();

            if (_tours == null || _tours.Count == 0)
                return new List<Tour>();

            var originalTour = _tours.FirstOrDefault(t => t.Id == originalTourId);
            if (originalTour?.Location == null)
                return new List<Tour>();

            // Pronađi sve ture koje:
            // 1. Nisu originalna tura
            // 2. Imaju istu lokaciju (grad i državu)  
            // 3. Imaju dovoljno slobodnih mesta
            return _tours.Where(t =>
                t.Id != originalTourId && // Nije originalna tura
                t.Location != null && // Ima lokaciju
                t.Location.City != null && t.Location.Country != null && // Lokacija ima grad i državu
                originalTour.Location.City != null && originalTour.Location.Country != null && // Originalna lokacija je validna
                t.Location.City.Trim().Equals(originalTour.Location.City.Trim(), StringComparison.OrdinalIgnoreCase) && // Isti grad
                t.Location.Country.Trim().Equals(originalTour.Location.Country.Trim(), StringComparison.OrdinalIgnoreCase) && // Ista država
                t.AvailableSpots >= requiredSpots // Ima dovoljno mesta
            ).ToList();
        }

        public Tour GetById(int id)
        {
            _tours = LoadToursWithLocations();
            return _tours?.FirstOrDefault(t => t.Id == id);
        }

        public Tour Save(Tour tour)
        {
            _tours = LoadToursWithLocations();
            tour.Id = NextId();
            _tours.Add(tour);
            _serializer.ToCSV(FilePath, _tours);
            return tour;
        }

        public int NextId()
        {
            _tours = LoadToursWithLocations();
            if (_tours == null || _tours.Count < 1)
            {
                return 1;
            }
            return _tours.Max(c => c.Id) + 1;
        }

        public void Delete(Tour tour)
        {
            _tours = LoadToursWithLocations();
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
            _tours = LoadToursWithLocations();
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