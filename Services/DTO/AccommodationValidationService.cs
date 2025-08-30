using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class AccommodationValidationService
    {
        public bool IsValid(AccommodationDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return false;
            if (!dto.MaxGuests.HasValue || dto.MaxGuests < 1) return false;
            if (!dto.MinReservationDays.HasValue || dto.MinReservationDays < 1) return false;
            if (dto.CancellationDeadlineDays < 1) return false;
            if (dto.ImagePaths != null)
            {
                foreach (var img in dto.ImagePaths)
                {
                    if (string.IsNullOrWhiteSpace(img.Path)) return false;
                }
            }
            return true;
        }
    }
}
