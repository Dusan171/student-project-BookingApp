using System;
using BookingApp.Domain;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class MyReservationViewModel
    {
        //podaci iz Reservation modela
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestsNumber     { get; set; }

        //podaci iz Accommodation modela
        public string AccommodationName { get; set; }

        //podaci iz Reschedule  Request modela
        public string RequestStatusText { get; set; }
        public string OwnerComment { get; set; }
        //logika za UI
        public bool IsRescheduleEnabled { get; set; }

        //cuvam originalnu rezervaciju ako zatreba
        public Reservation OriginalReservation { get; set; }
    }
}
