using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;

namespace BookingApp.Repositories
{

    public class ReservationGuestRepository

    {
        private const string FilePath = "../../Resources/Data/reservationGuests.csv";
        private readonly Serializer<ReservationGuest> _serializer;
        private List<ReservationGuest> _guests;

        public ReservationGuestRepository()
        {
            _serializer = new Serializer<ReservationGuest>();
            _guests = _serializer.FromCSV(FilePath);
        }

        public int NextId()
        {
            if (_guests.Count < 1)
                return 1;
            return _guests.Max(g => g.Id) + 1;
        }

        public List<ReservationGuest> GetAll()
        {
            return _guests;
        }

        public List<ReservationGuest> GetByReservationId(int reservationId)
        {
            return _guests.Where(g => g.ReservationId == reservationId).ToList();
        }

        public ReservationGuest GetById(int id)
        {
            return _guests.FirstOrDefault(g => g.Id == id);
        }

        public void SaveAll()
        {
            _serializer.ToCSV(FilePath, _guests);
        }

        public ReservationGuest Add(ReservationGuest guest)
        {
            guest.Id = NextId();
            _guests.Add(guest);
            SaveAll();
            return guest;
        }

        public void RemoveByReservationId(int reservationId)
        {
            _guests.RemoveAll(g => g.ReservationId == reservationId);
            SaveAll();
        }
    }
}
