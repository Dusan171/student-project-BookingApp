using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repository;

        public LocationService(ILocationRepository repository)
        {
            _repository = repository;
        }

        public List<LocationDTO> GetAllLocations()
        {
            return _repository.GetAll()
                      .Select(location => new LocationDTO(location))
                      .ToList();
        }

        public LocationDTO AddLocation(LocationDTO location)
        {
            return new LocationDTO(_repository.Save(location.ToLocation()));
        }

        public void DeleteLocation(LocationDTO location)
        {
            _repository.Delete(location.ToLocation());
        }

        public LocationDTO UpdateLocation(LocationDTO location)
        {
            return new LocationDTO(_repository.Update(location.ToLocation()));
        }
    }
}
