using BookingApp.Domain.Interfaces;
using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class AccommodationService
    {
        private readonly IAccommodationRepository _repository;

        
        public AccommodationService(IAccommodationRepository repository)
        {
            _repository = repository;
        }

        public List<Accommodation> GetAllAccommodations()
        {
            return _repository.GetAll();
        }

        public Accommodation GetAccommodationById(int id)
        {
            return _repository.GetById(id);
        }

        public Accommodation AddAccommodation(Accommodation accommodation)
        {
            // izmeni
            if (!accommodation.IsValid())
                throw new System.Exception("Accommodation is not valid");

            return _repository.Save(accommodation);
        }

        public void DeleteAccommodation(Accommodation accommodation)
        {
            _repository.Delete(accommodation);
        }

        public Accommodation UpdateAccommodation(Accommodation accommodation)
        {
            return _repository.Update(accommodation);
        }

        public List<Accommodation> GetAccommodationsByLocation(Location location)
        {
            return _repository.GetByLocation(location);
        }
    }
}
