using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationService
    {
        List<AccommodationDTO> GetAllAccommodations();
        AccommodationDTO GetAccommodationById(int id);
        AccommodationDTO AddAccommodation(AccommodationDTO accommodation);
        void DeleteAccommodation(AccommodationDTO accommodation);
        AccommodationDTO UpdateAccommodation(AccommodationDTO accommodation);
        List<AccommodationDTO> GetAccommodationsByLocation(LocationDTO location);
        bool RegisterAccommodation(AccommodationDTO accommodationDto);
        List<AccommodationDTO> GetAccommodationsByOwnerId(int ownerId);
        int GetActiveAccommodationsCount(int ownerId);
        int GetTotalAccommodationsCount(int ownerId);

        List<Accommodation> GetByOwnerId(int ownerId);
        string GetLocationString(int locationId);

    }
}
