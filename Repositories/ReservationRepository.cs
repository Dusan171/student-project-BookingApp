using System;
using System.Collections.Generic;
using System.IO; //za koristenje File.Exits i File.Create
using System.Linq; //za Where, Max, Any za filtriranje i obradu kolekcija
using BookingApp.Domain;
using BookingApp.Serializer;
using BookingApp.Utilities;

namespace BookingApp.Repositories
{
    public class ReservationRepository
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
        //ovo se izgleda nigdje ne koristi
        /*public void Add(Reservation reservation)
        {
            _reservations = _serializer.FromCSV(FilePath);
            reservation.Id = NextId();
            _reservations.Add(reservation);
            _serializer.ToCSV(FilePath, _reservations);
        }*/
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
       /* public void CreateReservation(Accommodation accommodation,DateTime startDate,DateTime endDate, int guestNumber, OccupiedDateRepository occupiedDateRepository)
        {
            //validacija poslovnih pravila
            if (guestNumber > accommodation.MaxGuests)
            {
                throw new Exception($"Max allowed guests: {accommodation.MaxGuests}");
            }
            int stayLength = (endDate - startDate).Days;
            if (stayLength < accommodation.MinReservationDays)
            {
                throw new Exception($"Minimum stay is {accommodation.MinReservationDays} days.");
            }
            //provjera preklapanja sa zauzetim datumima
            var occupiedDates = occupiedDateRepository.GetByAccommodationId(accommodation.Id);
            bool isOverlap = Enumerable.Range(0, stayLength).Select(offset => startDate.AddDays(offset).Date).Any(date => occupiedDates.Any(o => o.Date == date));

            if (isOverlap)
            {
                throw new Exception("Selected period is not available.");
            }

            //Kreiranje i cuvanje rezervacije
            var reservation = new Reservation
            {
                AccommodationId = accommodation.Id,
                GuestId = Session.CurrentUser.Id,
                StartDate = startDate,
                EndDate = endDate,
                GuestsNumber = guestNumber,
                Status = ReservationStatus.Active
            };
            Save(reservation); // Koristim postojeću Save metodu

            //Kreiranje i cuvanje zauzetih datuma
            List<OccupiedDate> occupiedDatesToSave = new List<OccupiedDate>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                occupiedDatesToSave.Add(new OccupiedDate
                {
                    // ID će biti dodeljen unutar OccupiedDateRepository
                    AccommodationId = accommodation.Id,
                    ReservationId = reservation.Id,
                    Date = date
                });
            }
            occupiedDateRepository.Save(occupiedDatesToSave);
        }*/
    }
}