using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain;
using BookingApp.Serializer;

namespace BookingApp.Repositories
{
    internal class TourReservationRepository
    {
        private const string FilePath = "C:/Users/PC/Desktop/5 semestar/sims-projekat/sims-ra-2025-group-7-team-b/Resources/Data/tourReservation.csv";
        private readonly Serializer<TourReservation> _serializer;
        private List<TourReservation> _reservations;

        public TourReservationRepository()
        {
            _serializer = new Serializer<TourReservation>();
            _reservations = _serializer.FromCSV(FilePath);
        }

        public List<TourReservation> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public TourReservation Save(TourReservation reservation)
        {
            reservation.Id = NextId();
            _reservations = _serializer.FromCSV(FilePath);
            _reservations.Add(reservation);
            _serializer.ToCSV(FilePath, _reservations);
            return reservation;
        }

        public int NextId()
        {
            _reservations = _serializer.FromCSV(FilePath);
            if (_reservations.Count < 1)
                return 1;
            return _reservations.Max(r => r.Id) + 1;
        }

        public void Delete(TourReservation reservation)
        {
            _reservations = _serializer.FromCSV(FilePath);
            TourReservation found = _reservations.Find(r => r.Id == reservation.Id);
            _reservations.Remove(found);
            _serializer.ToCSV(FilePath, _reservations);
        }

        public TourReservation Update(TourReservation reservation)
        {
            _reservations = _serializer.FromCSV(FilePath);
            TourReservation current = _reservations.Find(r => r.Id == reservation.Id);
            int index = _reservations.IndexOf(current);
            _reservations.Remove(current);
            _reservations.Insert(index, reservation);
            _serializer.ToCSV(FilePath, _reservations);
            return reservation;
        }

        public List<TourReservation> GetByTourist(int touristId)
        {
            _reservations = _serializer.FromCSV(FilePath);
            return _reservations.Where(r => r.TouristId == touristId).ToList();
        }

        public List<TourReservation> GetByTour(int tourId)
        {
            _reservations = _serializer.FromCSV(FilePath);
            return _reservations.Where(r => r.TourId == tourId).ToList();
        }
    }
}
