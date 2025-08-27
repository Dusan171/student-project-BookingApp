using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BookingApp.Repositories
{
    public class AccommodationRepository : IAccommodationRepository
    {

        private const string FilePath = "../../../Resources/Data/accommodation.csv";

        private readonly Serializer<Accommodation> _serializer;

        private List<Accommodation> _accommodations;

        public AccommodationRepository()
        {
            _serializer = new Serializer<Accommodation>();
            _accommodations = _serializer.FromCSV(FilePath);
        }

        public List<Accommodation> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public Accommodation GetById(int id)
        {
            _accommodations = _serializer.FromCSV(FilePath);
            return _accommodations.FirstOrDefault(a => a.Id == id);
        }
        public Accommodation Save(Accommodation accommodation)
        {
            accommodation.Id = NextId();
            _accommodations = _serializer.FromCSV(FilePath);
            _accommodations.Add(accommodation);
            _serializer.ToCSV(FilePath, _accommodations);
            return accommodation;
        }

        public int NextId()
        {
            _accommodations = _serializer.FromCSV(FilePath);
            if (_accommodations.Count < 1)
            {
                return 1;
            }
            return _accommodations.Max(c => c.Id) + 1;
        }

        public void Delete(Accommodation accommodation)
        {
            _accommodations = _serializer.FromCSV(FilePath);
            Accommodation founded = _accommodations.Find(a => a.Id == accommodation.Id);
            _accommodations.Remove(founded);
            _serializer.ToCSV(FilePath, _accommodations);
        }

        public Accommodation Update(Accommodation accommodation)
        {
            _accommodations = _serializer.FromCSV(FilePath);
            Accommodation current = _accommodations.Find(a => a.Id == accommodation.Id);
            int index = _accommodations.IndexOf(current);
            _accommodations.Remove(current);
            _accommodations.Insert(index, accommodation);      
            _serializer.ToCSV(FilePath, _accommodations);
            return accommodation;
        }

        public List<Accommodation> GetByLocation(Location location)
        {
            _accommodations = _serializer.FromCSV(FilePath);
            return _accommodations.FindAll(c => c.GeoLocation.Id == location.Id);
        }
    }
}

