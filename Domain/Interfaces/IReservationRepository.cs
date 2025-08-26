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
        public Reservation Save(Reservation reservation);
        public List<Reservation> GetByGuestId(int guestId);
        public Reservation GetById(int id);
        public void Delete(Reservation reservation);
        public Reservation Update(Reservation reservation);
        void Update(Reservation reservation);
    }
}
