using System.Collections.Generic;
using System.Linq;
using BookingApp.Model;
using BookingApp.Serializer;
using System.IO;

namespace BookingApp.Repository
{
    public class OccupiedDateRepository
    {
        private const string FilePath = "../../../Resources/Data/occupiedDates.csv";
        private readonly Serializer<OccupiedDate> _serializer;
        private List<OccupiedDate> _occupiedDates;

        public OccupiedDateRepository()
        {
            _serializer = new Serializer<OccupiedDate>();
            _occupiedDates = _serializer.FromCSV(FilePath);
        }
        public List<OccupiedDate> GetAll() 
        {
            return _occupiedDates;
        }
        public List<OccupiedDate> GetByAccommodationId(int accommodationId)
        {
            return _occupiedDates.Where(o => o.AccommodationId == accommodationId).ToList();
        }
        public void Save(List<OccupiedDate> newDates)
        {
            _occupiedDates.AddRange(newDates);
            _serializer.ToCSV(FilePath, _occupiedDates);
        }
        public int NextId()
        {
            return _occupiedDates.Count == 0 ? 1 : _occupiedDates.Max(o => o.Id) + 1;
        }
    }
}
