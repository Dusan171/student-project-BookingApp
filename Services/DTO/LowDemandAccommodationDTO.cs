namespace BookingApp.Services.DTO
{
    public class LowDemandAccommodationDTO
    {
        public int AccommodationId { get; set; }
        public string AccommodationName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int ReservationCount { get; set; }
        public double OccupancyRate { get; set; }
        public string Recommendation { get; set; }

        public LowDemandAccommodationDTO()
        {
            Recommendation = "Low performance - consider closing or relocating this property";
        }
    }
}