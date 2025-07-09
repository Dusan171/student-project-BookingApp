using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    internal class ReservationGuestRepository
    {
        private const string FilePath = "C:/Users/PC/Desktop/5 semestar/sims-projekat/sims-ra-2025-group-7-team-b/Resources/Data/reservationGuests.csv";

        private readonly Serializer<ReservationGuest> _serializer;

        private List<ReservationGuest> _guests;

        public ReservationGuestRepository()
        {
            _serializer = new Serializer<ReservationGuest>();
            _guests = _serializer.FromCSV(FilePath);
        }

        public List<ReservationGuest> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public ReservationGuest Save(ReservationGuest guest)
        {
            guest.Id = NextId();
            _guests = _serializer.FromCSV(FilePath);
            _guests.Add(guest);
            _serializer.ToCSV(FilePath, _guests);
            return guest;
        }

        public int NextId()
        {
            _guests = _serializer.FromCSV(FilePath);
            if (_guests.Count < 1)
            {
                return 1;
            }
            return _guests.Max(g => g.Id) + 1;
        }

        public void Delete(ReservationGuest guest)
        {
            _guests = _serializer.FromCSV(FilePath);
            ReservationGuest found = _guests.Find(g => g.Id == guest.Id);
            _guests.Remove(found);
            _serializer.ToCSV(FilePath, _guests);
        }

        public ReservationGuest Update(ReservationGuest guest)
        {
            _guests = _serializer.FromCSV(FilePath);
            ReservationGuest current = _guests.Find(g => g.Id == guest.Id);
            int index = _guests.IndexOf(current);
            _guests.Remove(current);
            _guests.Insert(index, guest);
            _serializer.ToCSV(FilePath, _guests);
            return guest;
        }

        public List<ReservationGuest> GetByReservationId(int reservationId)
        {
            _guests = _serializer.FromCSV(FilePath);
            return _guests.FindAll(g => g.ReservationId == reservationId);
        }
    }
}
