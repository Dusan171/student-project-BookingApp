using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;
using BookingApp.Domain.Interfaces;

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
            var savedLocation = _repository.Save(location.ToLocation());
            return new LocationDTO(savedLocation);
        }

        public void DeleteLocation(LocationDTO location)
        {
            _repository.Delete(location.ToLocation());
        }

        public LocationDTO UpdateLocation(LocationDTO location)
        {
            var updatedLocation = _repository.Update(location.ToLocation());
            return new LocationDTO(updatedLocation);
        }
    }
}