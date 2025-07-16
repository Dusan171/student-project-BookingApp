using BookingApp.Model;
using BookingApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Utilities
{
    public class NotificationsGenerator
    {
        public static List<Notification> Generate(List<Reservation> reservations, GuestReviewRepository guestReviewRepository)
        {
            var notifications = new List<Notification>();

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

        private static bool IsReservationFinished(Reservation reservation) => reservation.EndDate < DateTime.Now;

        private static bool IsReservationRated(Reservation reservation, GuestReviewRepository repo) =>
            repo.GetByReservationId(reservation).Any();

        private static bool IsWithinRatingPeriod(Reservation reservation)
        {
            int daysPassed = (DateTime.Now - reservation.EndDate).Days;
            return daysPassed >= 0 && daysPassed <= 5;
        }

        private static Notification NewNotification(Reservation reservation)
        {
            return new Notification
            {
                ReservationId= reservation.Id,
                GuestId = reservation.GuestId,
                Deadline = reservation.EndDate.AddDays(5),
                IsRead = false
            };
        }


    }
}
