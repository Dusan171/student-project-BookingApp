using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationRepository
    {
        List<Reservation> GetAll();
        void Add(Reservation reservation);
        List<Reservation> GetByGuestId(int guestId);
        int NextId();
        Reservation Save(Reservation reservation);
    }
}
