using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationImageRepository
    {
        List<AccommodationImage> GetAll();
        AccommodationImage Save(AccommodationImage image);
        void Delete(AccommodationImage image);
        AccommodationImage Update(AccommodationImage image);
        List<AccommodationImage> GetByAccommodation(Accommodation accommodation);

    }
}
