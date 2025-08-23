using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationService
    {
        List<Accommodation> GetAllAccommodations();
        Accommodation GetById(int id);
        Accommodation AddAccommodation(Accommodation accommodation);
        void DeleteAccommodation(Accommodation accommodation);
        Accommodation UpdateAccommodation(Accommodation accommodation);
        List<Accommodation> GetAccommodationsByLocation(Location location);
    }
}
