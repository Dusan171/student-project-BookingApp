using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface ILocationRepository
    {
        List<Location> GetAll();
        Location Save(Location location);
        void Delete(Location location);
        Location Update(Location location);
        Location? GetById(int id);
        //Location? GetByName(string city, string country);
    }
}
