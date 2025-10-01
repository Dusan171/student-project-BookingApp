using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Repositories
{
    public class OccupiedDateRepository : IOccupiedDateRepository
    {
        private const string FilePath = "../../../Resources/Data/occupiedDates.csv";
        private readonly Serializer<OccupiedDate> _serializer;

        public OccupiedDateRepository()
        {
            _serializer = new Serializer<OccupiedDate>();
        }
        public List<OccupiedDate> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }
        public List<OccupiedDate> GetByAccommodationId(int accommodationId)
        {
            var allDates = GetAll();
            return allDates.Where(o => o.AccommodationId == accommodationId).ToList();
        }
        public void Save(List<OccupiedDate> newDates)
        {
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
            var allDates = GetAll();
            return allDates.Any() ? allDates.Max(o => o.Id) + 1 : 1;
        }
        public void DeleteByReservationId(int reservationId)
        {
            var allDates = GetAll();

            var remainingDates = allDates.Where(d => d.ReservationId != reservationId).ToList();

            _serializer.ToCSV(FilePath, remainingDates);
        }
    }
}