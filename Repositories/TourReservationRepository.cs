using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
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

        public List<TourReservation> GetAll()
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations.ToList();
        }

        public TourReservation? GetById(int id)
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations.FirstOrDefault(tr => tr.Id == id);
        }

        public TourReservation Add(TourReservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));

            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            reservation.Id = GetNextId();
            _tourReservations.Add(reservation);
            SaveAll();
            return reservation;
        }

        public TourReservation Update(TourReservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));

            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
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
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            var reservation = _tourReservations.FirstOrDefault(tr => tr.Id == id);

            if (reservation != null)
            {
                _tourReservations.Remove(reservation);
                SaveAll();
            }
        }

        public void SaveAll()
        {
            _serializer.ToCSV(FilePath, _tourReservations);
        }

        public int GetNextId()
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations.Count == 0 ? 1 : _tourReservations.Max(tr => tr.Id) + 1;
        }

        public List<TourReservation> GetByTouristId(int touristId)
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations.Where(tr => tr.TouristId == touristId).ToList();
        }

        public List<TourReservation> GetByTourId(int tourId)
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations.Where(tr => tr.TourId == tourId).ToList();
        }

        public List<TourReservation> GetByStatus(TourReservationStatus status)
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations.Where(tr => tr.Status == status).ToList();
        }

        public List<TourReservation> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations.Where(tr =>
                tr.ReservationDate.Date >= startDate.Date &&
                tr.ReservationDate.Date <= endDate.Date).ToList();
        }

        public List<TourReservation> GetTodaysReservations()
        {
            var today = DateTime.Today;
            return GetByDateRange(today, today);
        }

        public List<TourReservation> GetCompletedReservationsByTourist(int touristId)
        {
            _tourReservations = _serializer.FromCSV(FilePath) ?? new List<TourReservation>();
            return _tourReservations
                .Where(tr => tr.TouristId == touristId && tr.Status == TourReservationStatus.COMPLETED)
                .ToList();
        }


        public List<TourReservation> GetReservationsByTourist(int touristId)
        {
            return GetByTouristId(touristId);
        }

        public List<TourReservation> GetCompletedUnreviewedReservationsByTourist(int touristId)
        {
            var completedReservations = GetCompletedReservationsByTourist(touristId);
            // Placeholder - trebalo bi proveriti postojanje review-a
            return completedReservations;
        }

        public List<TourReservation> GetReservationsForTourist(int touristId)
        {
            return _tourReservations.Where(r => r.TouristId == touristId).ToList();
        }
    }
}