using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationGuestRepository
    {
        List<ReservationGuest> GetAll();
        ReservationGuest? GetById(int id);
        ReservationGuest Add(ReservationGuest guest);
        ReservationGuest Update(ReservationGuest guest);
        void Delete(int id);
        void SaveAll();
        int GetNextId();

        List<ReservationGuest> GetByReservationId(int reservationId);
        void RemoveByReservationId(int reservationId);
        List<ReservationGuest> GetAppearedGuests();
        bool UpdateAppearanceStatus(int guestId, bool hasAppeared, int keyPointJoinedAt = -1);
    }
}