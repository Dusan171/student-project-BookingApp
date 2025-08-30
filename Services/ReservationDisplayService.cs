using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class ReservationDisplayService : IReservationDisplayService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IRescheduleRequestRepository _rescheduleRequestRepository;
        private readonly IAccommodationReviewService _accommodationReviewService;
        private readonly IGuestReviewService _guestReviewService;

        public ReservationDisplayService(IReservationRepository reservationRepository, IAccommodationRepository accommodationRepository, IRescheduleRequestRepository rescheduleRequestRepository, IAccommodationReviewService accommodationReviewService, IGuestReviewService guestReviewService)
        {
            _reservationRepository = reservationRepository;
            _accommodationRepository = accommodationRepository;
            _rescheduleRequestRepository = rescheduleRequestRepository;
            _accommodationReviewService = accommodationReviewService;
            _guestReviewService = guestReviewService;
        }

        public List<ReservationDetailsDTO> GetReservationsForGuest(int guestId)
        {
            var allAccommodations = _accommodationRepository.GetAll();

            return _reservationRepository.GetByGuestId(guestId)
                .OrderByDescending(r => r.StartDate)
                .Select(reservation => MapToDetailsDTO(reservation, allAccommodations))
                .ToList();
        }

        private ReservationDetailsDTO MapToDetailsDTO(Reservation reservation, List<Accommodation> allAccommodations)
        {
            var accommodation = allAccommodations.FirstOrDefault(a => a.Id == reservation.AccommodationId);
            var request = _rescheduleRequestRepository.GetByReservationId(reservation.Id);

            bool hasGuestRated = _accommodationReviewService.HasGuestRated(reservation.Id);

            return new ReservationDetailsDTO
            {
                ReservationId = reservation.Id,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                GuestsNumber = reservation.GuestsNumber,
                AccommodationName = accommodation?.Name ?? "N/A",
                RequestStatusText = request?.Status.ToString() ?? "Not requested",
                OwnerComment = request?.OwnerComment ?? "",
                OriginalReservation = reservation,

                IsRescheduleEnabled = CalculateIsRescheduleEnabled(reservation, request),
                IsRatingEnabled = CalculateIsRatingEnabled(reservation, hasGuestRated),
                IsGuestReviewVisible = CalculateIsGuestReviewVisible(reservation, hasGuestRated)
            };
        }

        private bool CalculateIsRescheduleEnabled(Reservation reservation, RescheduleRequest request)
        {
            bool hasPendingRequest = (request != null && request.Status == RequestStatus.Pending);
            return reservation.StartDate > DateTime.Now && !hasPendingRequest;
        }

        private bool CalculateIsRatingEnabled(Reservation reservation, bool hasGuestAlreadyRated)
        {
            bool isFinished = reservation.EndDate < DateTime.Now;
            bool isWithinReviewPeriod = (DateTime.Now - reservation.EndDate).TotalDays <= 5;
            return isFinished && isWithinReviewPeriod && !hasGuestAlreadyRated;
        }

        private bool CalculateIsGuestReviewVisible(Reservation reservation, bool hasGuestAlreadyRated)
        {
            if (!hasGuestAlreadyRated) return false;

            var reviewFromOwner = _guestReviewService.GetReviewForReservation(reservation.Id);
            return reviewFromOwner != null;
        }
    }
}