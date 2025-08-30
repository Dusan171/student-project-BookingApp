using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private const string FilePath = "../../../Resources/Data/location.csv";
        private readonly Serializer<Location> _serializer;
        private List<Location> _locations;

        public LocationRepository()
        {
            _serializer = new Serializer<Location>();
            _locations = _serializer.FromCSV(FilePath) ?? new List<Location>(); 
        }

        public List<Location> GetAll()
        {
            return _serializer.FromCSV(FilePath) ?? new List<Location>();
        }

        public Location? GetById(int id)
        {
            _locations = _serializer.FromCSV(FilePath) ?? new List<Location>(); 
            return _locations.FirstOrDefault(l => l.Id == id);
        }

        public Location? GetByName(string city, string country)
        {
            _locations = _serializer.FromCSV(FilePath) ?? new List<Location>(); 
            return _locations.FirstOrDefault(l =>
                l.City?.Equals(city, StringComparison.OrdinalIgnoreCase) == true &&
                l.Country?.Equals(country, StringComparison.OrdinalIgnoreCase) == true);
        }

        public Location Save(Location location)
        {
            _locations = _serializer.FromCSV(FilePath) ?? new List<Location>(); 
            Location? current = _locations.Find(c =>
                c.City?.Equals(location.City, StringComparison.OrdinalIgnoreCase) == true &&
                c.Country?.Equals(location.Country, StringComparison.OrdinalIgnoreCase) == true);

            if (current != null)
            {
                return current;
            }
            else
            {
                location.Id = NextId();
                _locations = _serializer.FromCSV(FilePath) ?? new List<Location>();
                _locations.Add(location);
                _serializer.ToCSV(FilePath, _locations); 
                return location;
            }
        }

        public int NextId()
        {
            _locations = _serializer.FromCSV(FilePath) ?? new List<Location>(); 
            if (_locations.Count < 1)
            {
                return 1;
            }
            return _locations.Max(c => c.Id) + 1;
        }

        public void Delete(Location location)
        {
            _locations = _serializer.FromCSV(FilePath) ?? new List<Location>(); 
            Location? founded = _locations.Find(c => c.Id == location.Id);
            if (founded != null)
            {
                _locations.Remove(founded);
                _serializer.ToCSV(FilePath, _locations); 
            }
        }

        public Location Update(Location location)
        {
            _locations = _serializer.FromCSV(FilePath) ?? new List<Location>(); 
            Location? current = _locations.Find(c => c.Id == location.Id);
            if (current != null)
            {
                int index = _locations.IndexOf(current);
                _locations.Remove(current);
                _locations.Insert(index, location);
                _serializer.ToCSV(FilePath, _locations); 
            }
            return location;
        }
    }
}