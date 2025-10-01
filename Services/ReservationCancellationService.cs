using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using System;
using System.Linq;

namespace BookingApp.Services
{
    public class ReservationCancellationService : IReservationCancellationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IOccupiedDateRepository _occupiedDateRepository;
        private readonly ISystemNotificationRepository _systemNotificationRepository;
        private readonly IUserRepository _userRepository;

        // Konstruktor prima sve zavisnosti koje su mu potrebne
        public ReservationCancellationService(IReservationRepository resRepo, IAccommodationRepository accRepo,
                                              IOccupiedDateRepository occRepo, ISystemNotificationRepository sysNotRepo,
                                              IUserRepository userRepo)
        {
            _reservationRepository = resRepo;
            _accommodationRepository = accRepo;
            _occupiedDateRepository = occRepo;
            _systemNotificationRepository = sysNotRepo;
            _userRepository = userRepo;
        }

        public void CancelReservation(int reservationId)
        {
            // Svaki korak je sada posebna, privatna metoda
            var reservation = FindAndValidateReservation(reservationId);
            CheckCancellationPolicy(reservation);

            UpdateReservationStatus(reservation);
            FreeUpOccupiedDates(reservationId);
            NotifyOwner(reservation);
        }

        private Reservation FindAndValidateReservation(int reservationId)
        {
            var reservation = _reservationRepository.GetById(reservationId);
            if (reservation == null)
                throw new InvalidOperationException("Reservation not found.");
            return reservation;
        }

        private void CheckCancellationPolicy(Reservation reservation)
        {
            var accommodation = _accommodationRepository.GetById(reservation.AccommodationId);
            if (DateTime.Now.Date >= reservation.StartDate.AddDays(-accommodation.CancellationDeadlineDays).Date)
                throw new InvalidOperationException($"Cancellation deadline has passed.");
        }

        private void UpdateReservationStatus(Reservation reservation)
        {
            reservation.Status = ReservationStatus.Cancelled;
            _reservationRepository.Update(reservation);
        }

        private void FreeUpOccupiedDates(int reservationId)
        {
            _occupiedDateRepository.DeleteByReservationId(reservationId);
        }

        private void NotifyOwner(Reservation reservation)
        {
            var owner = _userRepository.GetAll().FirstOrDefault(u => u.Role == UserRole.OWNER);
            if (owner == null) return;

            var guest = _userRepository.GetById(reservation.GuestId);
            var accommodation = _accommodationRepository.GetById(reservation.AccommodationId);

            string message = $"Guest '{guest?.Username}' cancelled the reservation for '{accommodation?.Name}' ({reservation.StartDate:dd.MM.yyyy} - {reservation.EndDate:dd.MM.yyyy}).";

            var notification = new SystemNotification
            {
                UserId = owner.Id,
                Message = message,
                IsRead = false,
                Timestamp = DateTime.Now
            };
            _systemNotificationRepository.Save(notification);
        }
    }
}