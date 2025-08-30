using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Domain.Model;

namespace BookingApp.Services
{
    public class RequestsDisplayService
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IReservationService _reservationService;

        public RequestsDisplayService(IAccommodationService accommodationService, IReservationService reservationService)
        {
            _accommodationService = accommodationService;
            _reservationService = reservationService;
        }

        public void ProcessAndSetRequestData(RescheduleRequestDTO request)
        {
            var reservation = _reservationService.GetById(request.ReservationId);
            if (reservation == null) return;
            request.OriginalStartDate = reservation.StartDate;
            request.OriginalEndDate = reservation.EndDate;

            var accommodation = _accommodationService.GetAccommodationById(reservation.AccommodationId);
            if (accommodation == null) return;
            request.AccommodationName = accommodation.Name;

            SetAccommodationAvailabilityStatus(request, accommodation.Id);
        }

        private void SetAccommodationAvailabilityStatus(RescheduleRequestDTO request, int accommodationId)
        {
            bool isAvailable = _reservationService.IsAccommodationAvailable(accommodationId, request.NewStartDate, request.NewEndDate);
            request.AvailabilityStatus = isAvailable ? "Available" : "Not available";
        }
    }
}