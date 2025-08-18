using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using System.IO;
using BookingApp.Domain;

namespace BookingApp.Repositories
{
    public class OccupiedDateRepository
    {
        private const string FilePath = "../../../Resources/Data/occupiedDates.csv";
        private readonly Serializer<OccupiedDate> _serializer;
       // private List<OccupiedDate> _occupiedDates;

        public OccupiedDateRepository()
        {
            _serializer = new Serializer<OccupiedDate>();
            //_occupiedDates = _serializer.FromCSV(FilePath);
        }
        public List<OccupiedDate> GetAll()
        {
            //return _occupiedDates;
            //ovo uvek cita sveze podatke
            return _serializer.FromCSV(FilePath);
        }
        public List<OccupiedDate> GetByAccommodationId(int accommodationId)
        {
            var allDates = GetAll();
            return allDates.Where(o => o.AccommodationId == accommodationId).ToList();
        }
        public void Save(List<OccupiedDate> newDates)
        {
            //_occupiedDates.AddRange(newDates);
            // _serializer.ToCSV(FilePath, _occupiedDates);
            //sada Save sama dodeljuje Id
            //_occupiedDates = GetAll();
            var allDates = GetAll();
            int nextId = NextId();

            foreach (var date in newDates) 
            {
                date.Id = nextId;
                allDates.Add(date);
                nextId++;
            }
            _serializer.ToCSV(FilePath, allDates);
        }
        public int NextId()
        {
            //return _occupiedDates.Count == 0 ? 1 : _occupiedDates.Max(o => o.Id) + 1;
            var allDates = GetAll();
            return allDates.Any() ? allDates.Max(o => o.Id) + 1 : 1;
        }
    }
}