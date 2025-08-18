using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain;
using BookingApp.Presentation.ViewModels;
using BookingApp.Repositories;
using BookingApp.Utilities;

namespace BookingApp.Services
{
    public class ReservationDisplayService
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly AccommodationRepository _accommodationRepository;
        private readonly RescheduleRequestRepository _rescheduleRequestRepository;

        public ReservationDisplayService(ReservationRepository reservationRepository, AccommodationRepository accommodationRepository, RescheduleRequestRepository rescheduleRequestRepository)
        {
            _reservationRepository = reservationRepository;
            _accommodationRepository = accommodationRepository;
            _rescheduleRequestRepository = rescheduleRequestRepository;
        }

        //sva ova logika je bila u View a to ne smije
        public List<MyReservationViewModel> GetReservationsForGuest(int guestId)
        {
            var allAccommodations = _accommodationRepository.GetAll();
            var myReservations = _reservationRepository.GetByGuestId(guestId)
                                                      .OrderByDescending(r => r.StartDate).ToList();

            var displayList = new List<MyReservationViewModel>();

            foreach (var reservation in myReservations)
            {
                var accommodation = allAccommodations.FirstOrDefault(a => a.Id == reservation.AccommodationId);
                var request = _rescheduleRequestRepository.GetByReservationId(reservation.Id);

                var viewModel = new MyReservationViewModel
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
                viewModel.IsRescheduleEnabled = reservation.StartDate > DateTime.Now && !hasPendingRequest;

                displayList.Add(viewModel);
            }

            return displayList;
        }
    }
}
