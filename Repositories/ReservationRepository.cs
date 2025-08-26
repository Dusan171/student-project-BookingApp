using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Serializer;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private const string FilePath = "..//..//../Resources/Data/reservations.csv";
        private readonly Serializer<Reservation> _serializer;

        private List<Reservation> _reservations;

        public ReservationRepository()
        {
            _serializer = new Serializer<Reservation>();
            _reservations = _serializer.FromCSV(FilePath);
        }

        public List<Reservation> GetAll()
        {
            return _reservations;
        }

        public List<Reservation> GetByGuestId(int guestId)
        {
            return _reservations.Where(r => r.GuestId == guestId).ToList();
        }

        public Reservation GetById(int id)
        {
            return _reservations.FirstOrDefault(r => r.Id == id);
        }

        public Reservation Save(Reservation reservation)
        {
            reservation.Id = NextId();
            _reservations.Add(reservation);

            _serializer.ToCSV(FilePath, _reservations);

            return reservation;
        }
        public void Update(Reservation reservation)
        {
            var allReservations = GetAll();
            var existingReservationIndex = allReservations.FindIndex(r => r.Id == reservation.Id);

            if (existingReservationIndex != -1)
            {
                allReservations[existingReservationIndex] = reservation;
                _serializer.ToCSV(FilePath, allReservations);
            }
        }
        public void Delete(Reservation reservation)
        {
            var existing = GetById(reservation.Id);
            if (existing != null)
            {
                _reservations.Remove(existing);
                _serializer.ToCSV(FilePath, _reservations);
            }
        }
        private int NextId()
        {
            return _reservations.Any() ? _reservations.Max(r => r.Id) + 1 : 1;
        }
    }
}