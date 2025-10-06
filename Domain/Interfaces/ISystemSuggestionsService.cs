using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces.ServiceInterfaces
{
    public interface ISystemSuggestionsService
    {
        List<HighDemandLocationDTO> GetHighDemandLocations(int ownerId);
        List<LowDemandAccommodationDTO> GetLowDemandAccommodations(int ownerId);
    }
}
