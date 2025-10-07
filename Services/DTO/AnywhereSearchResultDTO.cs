using BookingApp.Domain.Model; 

namespace BookingApp.Services.DTO
{
    public class AnywhereSearchResultDTO 
    {
        public AccommodationDetailsDTO Accommodation { get; set; }
        public string OfferedDateRange { get; set; }

        public AnywhereSearchResultDTO() { }

        public AnywhereSearchResultDTO(AccommodationDetailsDTO accommodation) 
        {
            Accommodation = accommodation;
        }
    }
}