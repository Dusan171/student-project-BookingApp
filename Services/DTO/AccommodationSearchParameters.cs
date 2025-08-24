namespace BookingApp.Services.DTO
{
    public class AccommodationSearchParameters
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Type { get; set; }
        public int MaxGuests    { get; set; }
        public int MinDays { get; set; }
    }
}