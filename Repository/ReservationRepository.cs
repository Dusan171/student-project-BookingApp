using System;
using System.Collections.Generic;
using System.Linq; //za Where, Max, Any za filtriranje i obradu kolekcija
using System.Text;
using System.Threading.Tasks;
using System.IO; //za koristenje File.Exits i File.Create
using BookingApp.Model;
using BookingApp.Serializer;

namespace BookingApp.Repository
{
    public class ReservationRepository
    {
        private const string FilePath = "..//..//../Resources/Data/reservations.csv";
        private readonly Serializer<Reservation> _serializer;//objekat koji zna da radi serijalizaciju i deserijalizaciju objekta
        private List<Reservation> _reservations; //lokalna kopija svih rezervacija (tokom rada se koristi kao cache)

        public ReservationRepository()
        {
            _serializer = new Serializer<Reservation>();
            if (!File.Exists(FilePath))
                File.Create(FilePath).Close();

            _reservations = _serializer.FromCSV(FilePath); //ucitavanje svih rezervacija iz fajla u memoriju 
        }

        public List<Reservation> GetAll()
        {
            _reservations = _serializer.FromCSV(FilePath);
            return _reservations;
        }
        public void Add(Reservation reservation)
        {
            _reservations = _serializer.FromCSV(FilePath);
            reservation.Id = NextId();
            _reservations.Add(reservation);
            _serializer.ToCSV(FilePath, _reservations);
        }
        public List<Reservation> GetByGuestId(int guestId)
        {
            return GetAll().Where(r => r.GuestId == guestId).ToList();
        }

        public int NextId()
        {
            return _reservations.Any() ? _reservations.Max(r => r.Id) + 1 : 1;
        }
    }
}
