using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationRepository
    {
        Accommodation GetById(int id);
        List<Accommodation> GetAll();
        Accommodation Save(Accommodation accommodation);
        void Delete(Accommodation accommodation);
        Accommodation Update(Accommodation accommodation);
        List<Accommodation> GetByLocation(Location location);
        List<Accommodation> GetByOwnerId(int ownerId);
    }
}
