using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class RescheduleRequestService : IRescheduleRequestService
    {
        private readonly IOccupiedDateRepository _occupiedDateRepository;
        private readonly IRescheduleRequestRepository _rescheduleRequestRepository;
        private readonly IAccommodationRepository _accommodationRepository;

        public RescheduleRequestService(IOccupiedDateRepository occupiedDateRepository, IRescheduleRequestRepository rescheduleRequestRepository, IAccommodationRepository accommodationRepository)
        {
            _occupiedDateRepository = occupiedDateRepository;
            _rescheduleRequestRepository = rescheduleRequestRepository;
            _accommodationRepository = accommodationRepository;
        }
        //prebaceno iz RescheduleRequestView.xaml.cs

        // /// Dohvata sve zauzete datume za smeštaj, ISKLJUČUJUĆI datume trenutne rezervacije.
        public List<DateTime> GetBlackoutDatesForReschedule(Reservation reservation)
        {
            var allOccupiedDates = _occupiedDateRepository.GetByAccommodationId(reservation.AccommodationId);
            var currentReservationDates = Enumerable.Range(0, (reservation.EndDate - reservation.StartDate).Days)
                                                    .Select(offset => reservation.StartDate.AddDays(offset).Date)
                                                    .ToHashSet(); // Korišćenje HashSet-a za brže pretrage

            return allOccupiedDates
                .Where(od => !currentReservationDates.Contains(od.Date.Date))
                .Select(od => od.Date)
                .ToList();
        }
        /// Kreira i čuva zahtev za pomeranje, nakon provere svih poslovnih pravila.
        /// 
        public void CreateRequest(Reservation reservation, DateTime newStartDate, DateTime newEndDate)
        {
            var accommodation = _accommodationRepository.GetAll().FirstOrDefault(a => a.Id == reservation.AccommodationId);
            if (accommodation == null)
            {
                throw new Exception("Associated accommodation could not be found.");
            }

            // 1. Validacija poslovnih pravila
            int stayLength = (newEndDate - newStartDate).Days;
            if (stayLength < accommodation.MinReservationDays)
            {
                throw new Exception($"Minimum stay is {accommodation.MinReservationDays} days.");
            }

            // 2. Provera dostupnosti
            var blackoutDates = GetBlackoutDatesForReschedule(reservation);
            bool isOverlap = Enumerable.Range(0, stayLength)
                .Select(offset => newStartDate.AddDays(offset).Date)
                .Any(date => blackoutDates.Contains(date));

            if (isOverlap)
            {
                throw new Exception("Selected period overlaps with another reservation and is not available.");
            }

            // 3. Kreiranje i čuvanje
            var newRequest = new RescheduleRequest
            {
                ReservationId = reservation.Id,
                GuestId = Session.CurrentUser.Id,
                NewStartDate = newStartDate,
                NewEndDate = newEndDate,
                Status = RequestStatus.Pending,
                IsSeenByGuest = false
            };
            _rescheduleRequestRepository.Save(newRequest);
        }
    }
}
