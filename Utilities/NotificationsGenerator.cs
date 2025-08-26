using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Repositories;

namespace BookingApp.Utilities
{
    public class NotificationsGenerator
    {
        public static List<NotificationDTO> Generate(List<ReservationDTO> reservations, IGuestReviewRepository guestReviewRepository)
        {
            var notifications = new List<NotificationDTO>();

            foreach (var reservation in reservations)
            {
                if (IsReservationFinished(reservation) &&
                    !IsReservationRated(reservation, guestReviewRepository) &&
                    IsWithinRatingPeriod(reservation))
                {
                    notifications.Add(NewNotification(reservation));
                }
            }

            return notifications;
        }

        private static bool IsReservationFinished(ReservationDTO reservation) => reservation.EndDate < DateTime.Now;

        private static bool IsReservationRated(ReservationDTO reservation, IGuestReviewRepository repo) =>
            repo.GetByReservationId(reservation.ToReservation()).Any();

        private static bool IsWithinRatingPeriod(ReservationDTO reservation)
        {
            int daysPassed = (DateTime.Now - reservation.EndDate).Days;
            return daysPassed >= 0 && daysPassed <= 5;
        }

        private static NotificationDTO NewNotification(ReservationDTO reservation)
        {
            return new NotificationDTO
            {
                ReservationId= reservation.Id,
                GuestId = reservation.GuestId,
                Deadline = reservation.EndDate.AddDays(5),
                IsRead = false
            };
        }
    }
}
