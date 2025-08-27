using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

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
            var myReservations = _reservationRepository.GetByGuestId(guestId)
                                                      .OrderByDescending(r => r.StartDate)
                                                      .ToList();

            var dtoList = new List<ReservationDetailsDTO>();

            foreach (var reservation in myReservations)
            {
                var accommodation = allAccommodations.FirstOrDefault(a => a.Id == reservation.AccommodationId);
                var request = _rescheduleRequestRepository.GetByReservationId(reservation.Id);

                var dto = new ReservationDetailsDTO
                {
                    ReservationId = reservation.Id,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    GuestsNumber = reservation.GuestsNumber,
                    AccommodationName = accommodation?.Name ?? "N/A", 
                    RequestStatusText = request?.Status.ToString() ?? "Not requested",
                    OwnerComment = request?.OwnerComment ?? "",
                    OriginalReservation = reservation
                };

                bool hasPendingRequest = (request != null && request.Status == RequestStatus.Pending);
                dto.IsRescheduleEnabled = reservation.StartDate > DateTime.Now && !hasPendingRequest;
                bool isReservationFinished = reservation.EndDate < DateTime.Now;
                bool isWithinReviewPeriod = (DateTime.Now - reservation.EndDate).TotalDays <= 5;
                bool hasGuestAlreadyRated = _accommodationReviewService.HasGuestRated(reservation.Id);

                dto.IsRatingEnabled = isReservationFinished && isWithinReviewPeriod && !hasGuestAlreadyRated;
                var reviewFromOwner = _guestReviewService.GetReviewsByReservation(new ReservationDTO(reservation));
                bool hasOwnerRatedGuest = reviewFromOwner != null;
                dto.IsGuestReviewVisible = hasGuestAlreadyRated && hasOwnerRatedGuest;
                dtoList.Add(dto);
            }
            return dtoList;
        }
    }
}
