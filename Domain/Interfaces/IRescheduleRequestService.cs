using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IRescheduleRequestService
    {
        public List<DateTime> GetBlackoutDatesForReschedule(Reservation reservation);
        public void CreateRequest(Reservation reservation, DateTime newStartDate, DateTime newEndDate);
    }
}
