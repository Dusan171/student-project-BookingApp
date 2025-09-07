using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TourReservationRepository : ITourReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/tourReservations.csv";
        private readonly Serializer<TourReservation> _serializer;
        private List<TourReservation> _tourReservations;

        public TourReservationRepository()
        {
            _serializer = new Serializer<TourReservation>();
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
        }

        private void Reload()
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
        }

        public List<TourReservation> GetAll()
        {
            Reload();
            return _tourReservations.ToList();
        }

        public TourReservation? GetById(int id)
        {
            Reload();
            return _tourReservations.FirstOrDefault(tr => tr.Id == id);
        }

        public TourReservation Add(TourReservation reservation)
        {
            if (reservation == null) throw new ArgumentNullException(nameof(reservation));

            Reload();
            reservation.Id = GetNextId();
            _tourReservations.Add(reservation);
            SaveAll();
            return reservation;
        }

        public TourReservation Update(TourReservation reservation)
        {
            if (reservation == null) throw new ArgumentNullException(nameof(reservation));

            
            var existing = _tourReservations.FirstOrDefault(tr => tr.Id == reservation.Id);
            if (existing != null)
            {
                int index = _tourReservations.IndexOf(existing);
                _tourReservations[index] = reservation;
                SaveAll();
            }
            return reservation;
        }

        public void Delete(int id)
        {
           
            var existing = _tourReservations.FirstOrDefault(tr => tr.Id == id);
            if (existing != null)
            {
                _tourReservations.Remove(existing);
                SaveAll();
            }
        }

        public void SaveAll()
        {
            _serializer.ToCSV(FilePath, _tourReservations);
        }

        public int GetNextId()
        {
            
            return _tourReservations.Count == 0 ? 1 : _tourReservations.Max(r => r.Id) + 1;
        }

        public List<TourReservation> GetByUserId(int userId)
        {
            Reload();
            return _tourReservations.Where(r => r.TouristId == userId).ToList();
        }

        public List<TourReservation> GetByTouristId(int touristId)
        {
            return GetByUserId(touristId);
        }

        public List<TourReservation> GetByTourId(int tourId)
        {
            Reload();
            var reservations = _tourReservations.Where(r => r.TourId == tourId).ToList();
            System.Diagnostics.Debug.WriteLine($"TourReservationRepository.GetByTourId({tourId}): našao {reservations.Count} rezervacija");
            return _tourReservations.Where(r => r.TourId == tourId).ToList();
        }

        public List<TourReservation> GetByStatus(TourReservationStatus status)
        {
            Reload();
            return _tourReservations.Where(r => r.Status == status).ToList();
        }

        public List<TourReservation> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            Reload();
            return _tourReservations
                .Where(r => r.ReservationDate.Date >= startDate.Date && r.ReservationDate.Date <= endDate.Date)
                .ToList();
        }

        public List<TourReservation> GetTodaysReservations()
        {
            var today = DateTime.Today;
            return GetByDateRange(today, today);
        }

        public List<TourReservation> GetCompletedReservationsByTourist(int touristId)
        {
            Reload();
            return _tourReservations
                .Where(r => r.TouristId == touristId && r.Status == TourReservationStatus.COMPLETED)
                .ToList();
        }

        public List<TourReservation> GetReservationsByTourist(int touristId)
        {
            return GetByTouristId(touristId);
        }

        public List<TourReservation> GetCompletedUnreviewedReservationsByTourist(int touristId)
        {
            // Ovde kasnije mogu dodati logiku provere review-a
            return GetCompletedReservationsByTourist(touristId);
        }

        public List<TourReservation> GetReservationsForTourist(int touristId)
        {
            return GetByTouristId(touristId);
        }
    }
}
