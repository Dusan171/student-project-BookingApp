using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces.ServiceInterfaces
{
    using BookingApp.Services.DTO;
    using System.Collections.Generic;

    public interface ILocationService
    {
        List<LocationDTO> GetAllLocations();
        LocationDTO AddLocation(LocationDTO location);
        void DeleteLocation(LocationDTO location);
        LocationDTO UpdateLocation(LocationDTO location);
    }
}