using BookingApp.Domain.Model;
using System.Linq;

namespace BookingApp.Services.DTO
{
    public class AccommodationDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LocationDTO GeoLocation { get; set; }
        public string Type { get; set; }
        public int? MaxGuests { get; set; }
        public int? MinReservationDays { get; set; }
        public int CancellationDeadlineDays { get; set; }

        public AccommodationDetailsDTO() { }

        public AccommodationDetailsDTO(Accommodation accommodation)
        {
            Id = accommodation.Id;
            Name = accommodation.Name;
            GeoLocation = new LocationDTO(accommodation.GeoLocation);
            Type = accommodation.Type.ToString();
            MaxGuests = accommodation.MaxGuests;
            MinReservationDays = accommodation.MinReservationDays;
            CancellationDeadlineDays = accommodation.CancellationDeadlineDays;
        }
    }
}