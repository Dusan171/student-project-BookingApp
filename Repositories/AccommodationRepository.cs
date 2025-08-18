using BookingApp.Domain;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BookingApp.Repositories
{
    public class AccommodationRepository
    {

        private const string FilePath = "../../../Resources/Data/accommodation.csv";

        private readonly Serializer<Accommodation> _serializer;

        //private List<Accommodation> _accommodations;

        public AccommodationRepository()
        {
            _serializer = new Serializer<Accommodation>();
            //_accommodations = _serializer.FromCSV(FilePath);
        }

        public List<Accommodation> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public Accommodation Save(Accommodation accommodation)
        {
            var accommodations = GetAll();
            accommodation.Id = NextId();
            //_accommodations = _serializer.FromCSV(FilePath);
            accommodations.Add(accommodation);
            _serializer.ToCSV(FilePath, accommodations);
            return accommodation;
        }

        public int NextId()
        {
            var accommodations = GetAll();
            //_accommodations = _serializer.FromCSV(FilePath);
            if (!accommodations.Any())
            {
                return 1;
            }
            return accommodations.Max(c => c.Id) + 1;
        }

        public void Delete(Accommodation accommodation)
        {
            //_accommodations = _serializer.FromCSV(FilePath);
            var accommodations = GetAll();
            Accommodation found = accommodations.Find(a => a.Id == accommodation.Id);
            //_accommodations.Remove(founded);
            if (found != null) 
            {
                accommodations.Remove(found);
                _serializer.ToCSV(FilePath, accommodations);
            }
        }

        public Accommodation Update(Accommodation accommodation)
        {
            //_accommodations = _serializer.FromCSV(FilePath);
            var accommodations = GetAll();
            Accommodation current = accommodations.Find(a => a.Id == accommodation.Id);
            if (current != null)
            {
                int index = accommodations.IndexOf(current);
                accommodations.Remove(current);
                accommodations.Insert(index, accommodation);
                _serializer.ToCSV(FilePath, accommodations);
            }
            return accommodation;
        }

        public List<Accommodation> GetByLocation(Location location)
        {
            //_accommodations = _serializer.FromCSV(FilePath);
            var accommodations = GetAll();
            return accommodations.FindAll(c => c.GeoLocation.Id == location.Id);
        }

    }
}