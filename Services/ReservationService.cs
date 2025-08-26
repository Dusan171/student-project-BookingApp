using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IOccupiedDateRepository _occupiedDateRepository;

        //Dependency Injection (to ce vjerovatno trebati prebaciti u posebnu klasu ili gdje vec)

        public ReservationService(IReservationRepository reservationRepository, IOccupiedDateRepository occupiedDateRepository)
        {
            _reservationRepository = reservationRepository;
            _occupiedDateRepository = occupiedDateRepository;
        }
        //logika rezervacije koja je prije bila u reservationRepository
        public Reservation Create(Accommodation accommodation, DateTime startDate, DateTime endDate, int guestNumber)
        {
            // Validacija poslovnih pravila
            ValidateReservationRules(accommodation, startDate, endDate, guestNumber);

            // Provera dostupnosti
            CheckForOverlappingDates(accommodation.Id, startDate, endDate);

            // Kreiranje i čuvanje rezervacije
            var reservation = new Reservation
            {
                AccommodationId = accommodation.Id,
                GuestId = Session.CurrentUser.Id,
                StartDate = startDate,
                EndDate = endDate,
                GuestsNumber = guestNumber,
                Status = ReservationStatus.Active
            };
            _reservationRepository.Save(reservation);

            // Kreiranje i čuvanje zauzetih datuma
            CreateOccupiedDates(accommodation.Id, reservation.Id, startDate, endDate);

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

        public List<ReservationDTO> GetAll()
        {
            return _reservationRepository.GetAll()   // vraća listu entiteta Reservation
                                         .Select(reservation => new ReservationDTO(reservation))  // mapiranje u DTO
                                         .ToList();
        }

    }
}