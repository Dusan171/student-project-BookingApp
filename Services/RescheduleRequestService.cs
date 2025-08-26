using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTOs;

namespace BookingApp.Services
{
    public class RescheduleRequestService : IRescheduleRequestService
    {
        private readonly IOccupiedDateRepository _occupiedDateRepository;
        private readonly IRescheduleRequestRepository _rescheduleRequestRepository;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IReservationRepository _reservationRepository;

        public RescheduleRequestService(IOccupiedDateRepository occupiedDateRepository, IRescheduleRequestRepository rescheduleRequestRepository, IAccommodationRepository accommodationRepository, IReservationRepository reservationRepository)
        {
            _occupiedDateRepository = occupiedDateRepository;
            _rescheduleRequestRepository = rescheduleRequestRepository;
            _accommodationRepository = accommodationRepository;
            _reservationRepository = reservationRepository;
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
        public void CreateRequest(CreateRescheduleRequestDTO requestDto)
        {
            // Dobijamo originalnu rezervaciju iz baze na osnovu ID-a iz DTO-a
            var reservation = _reservationRepository.GetAll().FirstOrDefault(r => r.Id == requestDto.ReservationId);
            if (reservation == null)
            {
                throw new Exception("Reservation to reschedule could not be found.");
            }

            var accommodation = _accommodationRepository.GetAll().FirstOrDefault(a => a.Id == reservation.AccommodationId);
            if (accommodation == null)
            {
                throw new Exception("Associated accommodation could not be found.");
            }

            //Validacija poslovnih pravila (koristimo podatke iz DTO-a)
            int stayLength = (requestDto.NewEndDate - requestDto.NewStartDate).Days;
            if (stayLength < accommodation.MinReservationDays)
            {
                throw new Exception($"Minimum stay is {accommodation.MinReservationDays} days.");
            }

            //Provera dostupnosti
            var blackoutDates = GetBlackoutDatesForReschedule(reservation);
            bool isOverlap = Enumerable.Range(0, stayLength)
                .Select(offset => requestDto.NewStartDate.AddDays(offset).Date)
                .Any(date => blackoutDates.Contains(date));

            if (isOverlap)
            {
                throw new Exception("Selected period overlaps with another reservation and is not available.");
            }

            //Kreiranje domenskog modela iz DTO-a i čuvanje
            var newRequest = new RescheduleRequest
            {
                ReservationId = requestDto.ReservationId,
                GuestId = Session.CurrentUser.Id,
                NewStartDate = requestDto.NewStartDate,
                NewEndDate = requestDto.NewEndDate,
                Status = RequestStatus.Pending,
                IsSeenByGuest = false
            };
            _rescheduleRequestRepository.Save(newRequest);
        }
    }
}
