using System.Collections.Generic;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class AccommodationValidationService : IAccommodationValidationService
    {
        public bool IsAccommodationValid(AccommodationDTO accommodation ,out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(accommodation.Name))
            {
                errorMessage = "Please enter a property name.";
                return false;
            }

            if (accommodation.GeoLocation == null ||
                string.IsNullOrWhiteSpace(accommodation.GeoLocation.City) ||
                string.IsNullOrWhiteSpace(accommodation.GeoLocation.Country))
            {
                errorMessage = "Please enter a valid location (City, Country).";
                return false;
            }

            if (string.IsNullOrWhiteSpace(accommodation.Type))
            {
                errorMessage = "Please select a property type.";
                return false;
            }

            if (!accommodation.MaxGuests.HasValue || accommodation.MaxGuests < 1)
            {
                errorMessage = "Maximum guests must be at least 1.";
                return false;
            }

            if (!accommodation.MinReservationDays.HasValue || accommodation.MinReservationDays < 1)
            {
                errorMessage = "Minimum reservation days must be at least 1.";
                return false;
            }

            if (accommodation.CancellationDeadlineDays < 1)
            {
                errorMessage = "Cancellation deadline must be at least 1 day.";
                return false;
            }

            return true;
        }
    }
}