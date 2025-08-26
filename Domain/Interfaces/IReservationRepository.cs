using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationRepository
    {
        public List<Reservation> GetAll();
        public int NextId();
        public Reservation Save(Reservation reservation);
        public List<Reservation> GetByGuestId(int guestId);
        void Update(Reservation reservation);
    }
}
