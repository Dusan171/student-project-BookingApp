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
        bool RegisterAccommodation(AccommodationDTO accommodation);
    }
}
