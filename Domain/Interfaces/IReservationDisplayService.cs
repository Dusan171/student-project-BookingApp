using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Presentation.ViewModel.Guest;

namespace BookingApp.Domain.Interfaces
{
    public interface IReservationDisplayService
    {
        public List<MyReservationViewModel> GetReservationsForGuest(int guestId);
    }
}
