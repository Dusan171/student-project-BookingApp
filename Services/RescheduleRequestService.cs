using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;

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
       /* public List<DateTime> GetBlackoutDatesForReschedule(ReservationDTO reservationDto)
        {
            if (reservationDto == null)
            {
                return new List<DateTime>();
            }

            var reservation = reservationDto.ToReservation();

            var allOccupiedDates = _occupiedDateRepository.GetByAccommodationId(reservation.AccommodationId);

            var currentReservationDates = Enumerable.Range(0, (reservation.EndDate - reservation.StartDate).Days)
                                                    .Select(offset => reservation.StartDate.AddDays(offset).Date)
                                                    .ToHashSet();

            return allOccupiedDates
                .Where(od => !currentReservationDates.Contains(od.Date.Date))
                .Select(od => od.Date)
                .ToList();
        }*/
        public void CreateRequest(RescheduleRequestDTO requestDto)
        {
            var reservation = _reservationRepository.GetById(requestDto.ReservationId);
            if (reservation == null) throw new InvalidOperationException("Reservation not found.");

            var accommodation = _accommodationRepository.GetById(reservation.AccommodationId);
            if (accommodation == null) throw new InvalidOperationException("Accommodation not found.");

            ValidateRequestRules(accommodation, requestDto.NewStartDate, requestDto.NewEndDate);

           // CheckForAvailability(reservation, requestDto.NewStartDate, requestDto.NewEndDate);

            var newRequest = requestDto.ToRequest(); 

            newRequest.Status = RequestStatus.Pending;
            newRequest.OwnerComment = string.Empty; 
            newRequest.IsSeenByGuest = false;

            _rescheduleRequestRepository.Save(newRequest);
        }
        private void ValidateRequestRules(Accommodation accommodation, DateTime newStart, DateTime newEnd)
        {
            if (newEnd <= newStart)
            {
                throw new ArgumentException("End date must be after start date.");
            }

            int stayLength = (newEnd - newStart).Days;
            if (stayLength < accommodation.MinReservationDays)
            {
                throw new InvalidOperationException($"Minimum stay is {accommodation.MinReservationDays} days.");
            }
        }
        /*private void CheckForAvailability(Reservation originalReservation, DateTime newStart, DateTime newEnd)
        {
            var reservationDtoForCheck = new ReservationDTO(originalReservation);
            var blackoutDates = GetBlackoutDatesForReschedule(reservationDtoForCheck);

            int stayLength = (newEnd - newStart).Days;
            bool isOverlap = Enumerable.Range(0, stayLength)
                .Select(offset => newStart.AddDays(offset).Date)
                .Any(date => blackoutDates.Contains(date));

            if (isOverlap)
            {
                throw new InvalidOperationException("Selected period overlaps with another reservation.");
            }
        }  */
        public List<RescheduleRequestDTO> GetAll() 
        {
            return _rescheduleRequestRepository.GetAll().Select(r => new RescheduleRequestDTO(r)).ToList(); 
        }
      
        public RescheduleRequestDTO GetById(int id) 
        { 
            return _rescheduleRequestRepository.GetAll().FirstOrDefault(r => r.Id == id) == null ? null : new RescheduleRequestDTO();
        }
        public void Update(RescheduleRequestDTO requestDto)
        {
            var request = requestDto.ToRequest();
            var allRequests = _rescheduleRequestRepository.GetAll();
            var existingRequestIndex = allRequests.FindIndex(r => r.Id == request.Id);

            if (existingRequestIndex != -1)
            {
                allRequests[existingRequestIndex] = request;
                UpdateReservation(request);
                _rescheduleRequestRepository.SaveAll(allRequests);
            }
        }
        private void UpdateReservation(RescheduleRequest request)
        {
            var originalReservation = _reservationRepository.GetById(request.ReservationId);
            if (originalReservation == null)
            {
                throw new InvalidOperationException("Reservation not found.");
            }

            originalReservation.StartDate = request.NewStartDate;
            originalReservation.EndDate = request.NewEndDate;

            _reservationRepository.Update(originalReservation);
        }
        public void Create(CreateRescheduleRequestDTO requestDto)
        {
            var reservation = _reservationRepository.GetById(requestDto.ReservationId);
            if (reservation == null)
                throw new InvalidOperationException("Reservation to reschedule could not be found.");

            var accommodation = _accommodationRepository.GetById(reservation.AccommodationId);
            if (accommodation == null)
                throw new InvalidOperationException("Associated accommodation could not be found.");

            ValidateRequestRules(accommodation, requestDto.NewStartDate, requestDto.NewEndDate);
           // CheckForAvailability(reservation, requestDto.NewStartDate, requestDto.NewEndDate);

            var newRequest = new RescheduleRequest
            {
                ReservationId = requestDto.ReservationId,
                GuestId = requestDto.GuestId,
                NewStartDate = requestDto.NewStartDate,
                NewEndDate = requestDto.NewEndDate,
                Status = RequestStatus.Pending,
                IsSeenByGuest = false,
                OwnerComment = string.Empty
            };

            _rescheduleRequestRepository.Save(newRequest);
        }
    }
}
