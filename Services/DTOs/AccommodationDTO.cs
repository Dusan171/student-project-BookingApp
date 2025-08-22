using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services.DTOs
{
    public class AccommodationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
       // public string City { get; set; }
       // public string Country { get; set; }
       public Location GeoLocation { get; set; }
        public string Type { get; set; }
        public int MaxGuests { get; set; }
        public int MinReservationDays { get; set; }
        public int CancellationDeadlineDays { get; set; }
        public List<string> ImagePaths { get; set; }

        public AccommodationDTO()
        {
            ImagePaths = new List<string>();
        }

        public AccommodationDTO(Accommodation accommodation)
        {
            Id = accommodation.Id;
            Name = accommodation.Name;
            GeoLocation = accommodation.GeoLocation;
            Type = accommodation.Type.ToString();
            MaxGuests = accommodation.MaxGuests ?? 0;
            MinReservationDays = accommodation.MinReservationDays ?? 0;
            CancellationDeadlineDays = accommodation.CancellationDeadlineDays;
            ImagePaths = accommodation.Images?.Select(img => img.Path).ToList() ?? new List<string>();
        }

        public Accommodation ToAccommodation()
        {
            Enum.TryParse<AccommodationType>(Type, out var accommodationType);

            return new Accommodation
            {
                Id = this.Id,
                Name = this.Name,
                GeoLocation = this.GeoLocation,
                Type = accommodationType,
                MaxGuests = this.MaxGuests,
                MinReservationDays = this.MinReservationDays,
                CancellationDeadlineDays = this.CancellationDeadlineDays,
                Images = this.ImagePaths.Select(path => new AccommodationImage { Path = path }).ToList()
            };
        }
    }
}
