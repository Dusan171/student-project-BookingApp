namespace BookingApp.Services.DTO 
{
    public class CreateAccommodationReviewDTO
    {
        public int ReservationId { get; set; }
        public int CleanlinessRating { get; set; }
        public int OwnerRating { get; set; }
        public string Comment { get; set; }
        public string ImagePaths { get; set; }
    }
}