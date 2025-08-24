using System;
using System.Collections.Generic;
using System.IO; //za koristenje File.Exits i File.Create
using System.Linq; //za Where, Max, Any za filtriranje i obradu kolekcija
using BookingApp.Domain;
using BookingApp.Serializer;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private const string FilePath = "..//..//../Resources/Data/reservations.csv";
        private readonly Serializer<Reservation> _serializer;//objekat koji zna da radi serijalizaciju i deserijalizaciju objekta
        private List<Reservation> _reservations; //lokalna kopija svih rezervacija (tokom rada se koristi kao cache)

        public ReservationRepository()
        {
            _serializer = new Serializer<Reservation>();
           // if (!File.Exists(FilePath))
              //  File.Create(FilePath).Close();

            _reservations = _serializer.FromCSV(FilePath); //ucitavanje svih rezervacija iz fajla u memoriju 
        }

        public List<Reservation> GetAll()
        {
            //_reservations = _serializer.FromCSV(FilePath);
            return _serializer.FromCSV(FilePath);
        }
        public List<Reservation> GetByGuestId(int guestId)
        {
            return GetAll().Where(r => r.GuestId == guestId).ToList();
        }

        public int NextId()
        {
            // Osiguravam da uvek radim sa najsvežijim podacima pre generisanja ID-a
            //_reservations = _serializer.FromCSV(FilePath);
            _reservations = GetAll();
            return _reservations.Any() ? _reservations.Max(r => r.Id) + 1 : 1;
        }
        //potrebna za obavljanje rezervacije
        public Reservation Save(Reservation reservation)
        {
            _reservations = GetAll();
            reservation.Id = NextId();
            //_reservations = _serializer.FromCSV(FilePath);
            _reservations.Add(reservation);
            _serializer.ToCSV(FilePath, _reservations);
            return reservation;
        }
    }
}