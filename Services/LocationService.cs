using BookingApp.Domain.Interfaces;
using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repository;

        public LocationService(ILocationRepository repository)
        {
            _repository = repository;
        }

        public List<Location> GetAllLocations()
        {
            return _repository.GetAll();
        }

        public Location AddLocation(Location location)
        {
            return _repository.Save(location);
        }

        public void DeleteLocation(Location location)
        {
            _repository.Delete(location);
        }

        public Location UpdateLocation(Location location)
        {
            return _repository.Update(location);
        }
    }
}
