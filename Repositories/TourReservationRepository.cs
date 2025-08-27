using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TourReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/tourReservations.csv";
        private readonly Serializer<TourReservation> _serializer;
        private List<TourReservation> _tourReservations;
        private readonly ReservationGuestRepository _guestRepository;

        public TourReservationRepository()
        {
            _serializer = new Serializer<TourReservation>();
            _tourReservations = _serializer.FromCSV(FilePath);
            _guestRepository = new ReservationGuestRepository();
            LoadGuests();
        }

        private void LoadGuests()
        {
            foreach (var reservation in _tourReservations)
            {
                reservation.Guests = _guestRepository.GetByReservationId(reservation.Id);
            }
        }

        // DODANA metoda za generisanje ID-a
        public int NextId()
        {
            if (_tourReservations.Count < 1)
                return 1;
            return _tourReservations.Max(r => r.Id) + 1;
        }

        public List<TourReservation> GetAll()
        {
            return _tourReservations;
        }

        public List<TourReservation> GetByTouristId(int touristId)
        {
            return _tourReservations.Where(tr => tr.TouristId == touristId).ToList();
        }

        // DODANA metoda koju traži TourSearch.xaml.cs
        public List<TourReservation> GetReservationsByTourist(int touristId)
        {
            return GetByTouristId(touristId);
        }

        public List<TourReservation> GetTodaysReservations()
        {
            var today = DateTime.Today;
            return _tourReservations
                .Where(tr => tr.ReservationDate.Date == today)
                .ToList();
        }

        // IZMENA: Dodato generisanje ID-a pre dodavanja
        public TourReservation Add(TourReservation reservation)
        {
            reservation.Id = NextId();
            _tourReservations.Add(reservation);
            _serializer.ToCSV(FilePath, _tourReservations);

            foreach (var guest in reservation.Guests)
            {
                guest.ReservationId = reservation.Id;
                _guestRepository.Add(guest);
            }
            return reservation;
        }

        public void SaveAll()
        {
            _serializer.ToCSV(FilePath, _tourReservations);
        }
    }
}