using System.Windows.Media;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Utilities;

namespace BookingApp.Services.DTO
{
    public class TourDTO
    {
        public int Id { get; set; }
        public int GuideId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LocationText { get; set; } = string.Empty;
        public string DurationText { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxTourists { get; set; }
        public int ReservedSpots { get; set; }
        public int AvailableSpots { get; set; }
        public string AvailableSpotsText { get; set; } = string.Empty;
        public string GuideName { get; set; } = string.Empty;

        // COLOR PROPERTIES FOR UI
        public Brush AvailableSpotsColor { get; set; } = Brushes.Black;
        public Brush ReserveButtonColor { get; set; } = Brushes.Gray;
        public bool CanReserve { get; set; }

        // ORIGINAL OBJECT FOR BUSINESS OPERATIONS
        public Tour OriginalTour { get; set; } = new Tour();

        public static TourDTO FromDomain(Tour tour)
        {
            var availableSpots = tour.MaxTourists - tour.ReservedSpots;
            availableSpots = availableSpots < 0 ? 0 : availableSpots;

            return new TourDTO
            {
                Id = tour.Id,
                GuideId = Session.CurrentUser.Id,
                Name = tour.Name ?? "Nepoznata tura",
                LocationText = GetLocationText(tour.Location),
                DurationText = $"{tour.DurationHours}h",
                Language = tour.Language ?? "Nepoznat jezik",
                Description = tour.Description ?? "Nema opisa",
                MaxTourists = tour.MaxTourists,
                ReservedSpots = tour.ReservedSpots,
                AvailableSpots = availableSpots,
                AvailableSpotsText = $"Slobodnih mesta: {availableSpots}/{tour.MaxTourists}",
                GuideName = GetGuideName(tour.Guide),

                // UI PROPERTIES
                AvailableSpotsColor = availableSpots > 0 ? Brushes.Green : Brushes.Red,
                ReserveButtonColor = availableSpots > 0 ?
                    new SolidColorBrush(Color.FromRgb(46, 134, 193)) :
                    new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                CanReserve = availableSpots > 0,

                OriginalTour = tour
            };
        }

        private static string GetLocationText(Location? location)
        {
            return location != null ? $"{location.City}, {location.Country}" : "Nepoznata lokacija";
        }

        private static string GetGuideName(User? guide)
        {
            if (guide != null)
            {
                return $"{guide.FirstName} {guide.LastName}".Trim();
            }
            return "Nepoznat vodič";
        }

        public void RefreshAvailability(ITourReservationService reservationService)
        {
            var availableSpots = reservationService.GetAvailableSpotsForTour(this.Id);
            this.AvailableSpots = availableSpots;
            this.AvailableSpotsText = $"Slobodnih mesta: {availableSpots}";
            this.AvailableSpotsColor = availableSpots > 0 ? Brushes.Green : Brushes.Red;
        }
    }
}