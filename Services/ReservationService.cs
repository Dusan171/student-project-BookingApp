using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTOs;

namespace BookingApp.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IOccupiedDateRepository _occupiedDateRepository;
        private readonly IAccommodationRepository _accommodationRepository;

        //Dependency Injection (to ce vjerovatno trebati prebaciti u posebnu klasu ili gdje vec)

        public ReservationService(IReservationRepository reservationRepository, IOccupiedDateRepository occupiedDateRepository, IAccommodationRepository accommodationRepository)
        {
            _reservationRepository = reservationRepository;
            _occupiedDateRepository = occupiedDateRepository;
            _accommodationRepository = accommodationRepository;
        }
        //logika rezervacije koja je prije bila u reservationRepository
        public Reservation Create(CreateReservationDTO reservationDto)
        {
            // Moramo dobiti Accommodation objekat da bismo proverili pravila
            var accommodation = _accommodationRepository.GetAll().FirstOrDefault(a => a.Id == reservationDto.AccommodationId);
            if (accommodation == null)
            {
                throw new Exception("Cannot create reservation for an unknown accommodation.");
            }

            // Validacija poslovnih pravila
            ValidateReservationRules(accommodation, reservationDto.StartDate, reservationDto.EndDate, reservationDto.GuestsNumber);

            // Provera dostupnosti
            CheckForOverlappingDates(accommodation.Id, reservationDto.StartDate, reservationDto.EndDate);

            // Kreiranje domenskog modela iz DTO-a
            var reservation = new Reservation
            {
                AccommodationId = reservationDto.AccommodationId,
                GuestId = Session.CurrentUser.Id,
                StartDate = reservationDto.StartDate,
                EndDate = reservationDto.EndDate,
                GuestsNumber = reservationDto.GuestsNumber,
                Status = ReservationStatus.Active
            };
            _reservationRepository.Save(reservation);

            // Kreiranje zauzetih datuma
            CreateOccupiedDates(accommodation.Id, reservation.Id, reservationDto.StartDate, reservationDto.EndDate);

            return reservation;
        }
        private void ValidateReservationRules(Accommodation accommodation, DateTime startDate, DateTime endDate, int guestNumber)
        {
            if (guestNumber > accommodation.MaxGuests)
            {
                throw new Exception($"Max allowed guests: {accommodation.MaxGuests}");
            }
            int stayLength = (endDate - startDate).Days;
            if (stayLength < accommodation.MinReservationDays)
            {
                throw new Exception($"Minimum stay is {accommodation.MinReservationDays} days.");
            }
        }
        private void CheckForOverlappingDates(int accommodationId, DateTime startDate, DateTime endDate)
        {
            var occupiedDates = _occupiedDateRepository.GetByAccommodationId(accommodationId);
            int stayLength = (endDate - startDate).Days;
            bool isOverlap = Enumerable.Range(0, stayLength)
                .Select(offset => startDate.AddDays(offset).Date)
                .Any(date => occupiedDates.Any(o => o.Date == date));

            if (isOverlap)
            {
                throw new Exception("Selected period is not available.");
            }
        }
        private void CreateOccupiedDates(int accommodationId, int reservationId, DateTime startDate, DateTime endDate)
        {
            List<OccupiedDate> occupiedDatesToSave = new List<OccupiedDate>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                occupiedDatesToSave.Add(new OccupiedDate
                {
                    AccommodationId = accommodationId,
                    ReservationId = reservationId,
                    Date = date
                });
            }
            _occupiedDateRepository.Save(occupiedDatesToSave);
        }
        public List<DateTime> GetOccupiedDatesForAccommodation(int accommodationId)
        {
            // 1. Pozivamo repozitorijum da dobavimo listu ZAUZETIH DATUMA
            var occupiedDateObjects = _occupiedDateRepository.GetByAccommodationId(accommodationId);

            // 2. Koristimo LINQ (.Select) da iz svakog 'OccupiedDate' objekta
            //    izvučemo samo 'Date' svojstvo (koje je tipa DateTime).
            // 3. .ToList() pretvara rezultat u listu koju vraćamo.
            return occupiedDateObjects.Select(occupiedDate => occupiedDate.Date).ToList();
        }
    }
}
