using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Utilities;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTOs;
using BookingApp.Services.DTO;
using BookingApp.Repositories;

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


        public List<DateTime> GetBlackoutDatesForReschedule(Reservation reservation)
        {
            var allOccupiedDates = _occupiedDateRepository.GetByAccommodationId(reservation.AccommodationId);
            var currentReservationDates = Enumerable.Range(0, (reservation.EndDate - reservation.StartDate).Days)
                                                    .Select(offset => reservation.StartDate.AddDays(offset).Date)
                                                    .ToHashSet();

            return allOccupiedDates
                .Where(od => !currentReservationDates.Contains(od.Date.Date))
                .Select(od => od.Date)
                .ToList();
        }

        public void CreateRequest(RescheduleRequestDTO requestDto)
        {
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

            int stayLength = (requestDto.NewEndDate - requestDto.NewStartDate).Days;
            if (stayLength < accommodation.MinReservationDays)
            {
                throw new Exception($"Minimum stay is {accommodation.MinReservationDays} days.");
            }

            var blackoutDates = GetBlackoutDatesForReschedule(reservation);
            bool isOverlap = Enumerable.Range(0, stayLength)
                .Select(offset => requestDto.NewStartDate.AddDays(offset).Date)
                .Any(date => blackoutDates.Contains(date));

            if (isOverlap)
            {
                throw new Exception("Selected period overlaps with another reservation and is not available.");
            }

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
      

            
        public List<RescheduleRequestDTO> GetAll() 
        {
            return _rescheduleRequestRepository.GetAll().Select(r => new RescheduleRequestDTO(r)).ToList(); 
        }
      
        public RescheduleRequestDTO GetById(int id) 
        { 
            return _rescheduleRequestRepository.GetAll().FirstOrDefault(r => r.Id == id) == null ? null : new RescheduleRequestDTO(_rescheduleRequestRepository.GetAll().FirstOrDefault(r => r.Id == id));
        }
        public void Update(RescheduleRequestDTO requestDto)
        {
            var request = requestDto.ToRequest();
            var allRequests = _rescheduleRequestRepository.GetAll();
            var existingRequestIndex = allRequests.FindIndex(r => r.Id == request.Id);

            if (existingRequestIndex != -1)
            {
                allRequests[existingRequestIndex] = request;
                _rescheduleRequestRepository.SaveAll(allRequests);
            }
        }
    }
    
}
