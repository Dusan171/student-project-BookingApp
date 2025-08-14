using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain
{
    public class Notification
    {
        public int ReservationId { get; set; }
        public int GuestId { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsRead { get; set; } = false;

    }
}
