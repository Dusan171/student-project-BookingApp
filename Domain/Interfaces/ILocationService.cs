using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface ILocationService
    {
        List<Location> GetAllLocations();
        Location AddLocation(Location location);
        void DeleteLocation(Location location);
        Location UpdateLocation(Location location);
    }
}
