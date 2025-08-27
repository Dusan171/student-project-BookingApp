using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Model
{
    public class TouristAttendance
    {
        public int GuestId { get; set; }
        public int TourId { get; set; }
        public bool HasAppeared { get; set; }
        public int KeyPointJoinedAt { get; set; }
    }
}
