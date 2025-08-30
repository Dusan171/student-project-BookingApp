using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class AccommodationValidationService
    {
        private bool IsLocationValid(LocationDTO location)
        {
            return location != null &&
                   !string.IsNullOrWhiteSpace(location.City) &&
                   !string.IsNullOrWhiteSpace(location.Country);
        }
        public bool IsValid(AccommodationDTO dto)
        {
            return AreBasicDetailsValid(dto) && AreImagesValid(dto);
        }
        private bool AreBasicDetailsValid(AccommodationDTO dto)
        {
            return AreCapacitiesValid(dto)&&NameAndLocationValidation(dto);
        }
        private bool AreImagesValid(AccommodationDTO accommodation)
        {
            if (accommodation.ImagePaths != null)
            {
                foreach (var img in accommodation.ImagePaths)
                {
                    if (string.IsNullOrWhiteSpace(img.Path)) return false;
                }
            }
            return true;
        }
        private bool AreCapacitiesValid(AccommodationDTO accommodation)
        {
            if (!accommodation.MaxGuests.HasValue || accommodation.MaxGuests < 1) return false;
            if (!accommodation.MinReservationDays.HasValue || accommodation.MinReservationDays < 1) return false;
            
            return true;
        }
        private bool NameAndLocationValidation(AccommodationDTO accommodation)
        {
            if (string.IsNullOrWhiteSpace(accommodation.Name)) return false;

            if (!IsLocationValid(accommodation.GeoLocation)) return false;

            if (accommodation.CancellationDeadlineDays < 1) return false;

            return true;
        }
    }
}
