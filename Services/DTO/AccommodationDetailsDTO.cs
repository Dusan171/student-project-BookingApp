using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookingApp.Domain.Model;

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
        public List<string> ImagePaths { get; set; }

        public AccommodationDetailsDTO() 
        {
            ImagePaths = new List<string>();
        }
        public string FirstImagePath
        {
            get
            {
                if (ImagePaths == null || ImagePaths.Count == 0) return null;

                var path = ImagePaths[0];

                if (!Path.IsPathRooted(path))
                {
                    return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/Images/", path));
                }

                return path;
            }
        }

        public AccommodationDetailsDTO(Accommodation accommodation)
        {
            Id = accommodation.Id;
            Name = accommodation.Name;
            GeoLocation = new LocationDTO(accommodation.GeoLocation);
            Type = accommodation.Type.ToString();
            MaxGuests = accommodation.MaxGuests;
            MinReservationDays = accommodation.MinReservationDays;
            CancellationDeadlineDays = accommodation.CancellationDeadlineDays;
            ImagePaths = accommodation.Images.Select(img => img.Path).ToList();
        }
    }
}