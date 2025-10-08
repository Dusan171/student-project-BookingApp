using BookingApp.Domain.Model;

namespace BookingApp.Services.Validation
{
    public class TourDataValidator
    {
        public bool IsValidTour(Tour tour) => 
            tour != null && !string.IsNullOrWhiteSpace(tour.Name) && tour.Location != null && tour.MaxTourists > 0 && tour.DurationHours > 0;
    }
}