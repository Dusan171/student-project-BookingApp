using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationService
    {
        public Reservation Create(Accommodation accommodation, DateTime startDate, DateTime endDate, int guestNumber);
    }
}
