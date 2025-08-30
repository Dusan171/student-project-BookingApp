using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ReservationGuestRepository : IReservationGuestRepository
    {
        private const string FilePath = "../../../Resources/Data/reservationGuests.csv";
        private readonly Serializer<ReservationGuest> _serializer;
        private List<ReservationGuest> _guests;

        public ReservationGuestRepository()
        {
            _serializer = new Serializer<ReservationGuest>();
            _guests = _serializer.FromCSV(FilePath) ?? new List<ReservationGuest>();
        }

        private void SaveAll() => _serializer.ToCSV(FilePath, _guests);

        private void Reload() => _guests = _serializer.FromCSV(FilePath) ?? new List<ReservationGuest>();

        public List<ReservationGuest> GetAll() => _guests.ToList();

        public ReservationGuest? GetById(int id) => _guests.FirstOrDefault(g => g.Id == id);

        public List<ReservationGuest> GetByReservationId(int reservationId)
            => _guests.Where(g => g.ReservationId == reservationId).ToList();

        public List<ReservationGuest> GetAppearedGuests() 
            => _guests.Where(g => g.HasAppeared).ToList();

        public ReservationGuest Add(ReservationGuest guest)
        {
            if (guest == null) throw new ArgumentNullException(nameof(guest));

            guest.Id = GetNextId();
            _guests.Add(guest);
            SaveAll();
            return guest;
        }

        public ReservationGuest Update(ReservationGuest guest)
        {
            if (guest == null) throw new ArgumentNullException(nameof(guest));

            var index = _guests.FindIndex(g => g.Id == guest.Id);
            if (index >= 0)
            {
                _guests[index] = guest;
                SaveAll();
            }
            return guest;
        }

        public bool UpdateAppearanceStatus(int guestId, bool hasAppeared, int keyPointJoinedAt = -1)
        {
            var guest = _guests.FirstOrDefault(g => g.Id == guestId);
            if (guest != null)
            {
                guest.HasAppeared = hasAppeared;
                guest.KeyPointJoinedAt = keyPointJoinedAt;
                SaveAll();
                return true;
            }
            return false;
        }

        public void Delete(int id)
        {
            var guest = _guests.FirstOrDefault(g => g.Id == id);
            if (guest != null)
            {
                _guests.Remove(guest);
                SaveAll();
            }
        }

        public void RemoveByReservationId(int reservationId)
        {
            _guests.RemoveAll(g => g.ReservationId == reservationId);
            SaveAll();
        }

        public int GetNextId() => _guests.Count == 0 ? 1 : _guests.Max(g => g.Id) + 1;
    }
}
