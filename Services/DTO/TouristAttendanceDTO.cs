using System;

namespace BookingApp.DTO
{
    public class TouristAttendanceDTO
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public int TourId { get; set; }
        public DateTime StartTourTime { get; set; }
        public bool HasAppeared { get; set; }
        public int KeyPointJoinedAt { get; set; }
    }
}
