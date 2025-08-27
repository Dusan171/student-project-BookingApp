using BookingApp.Domain.Interfaces;
using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;
using BookingApp.Repositories;

namespace BookingApp.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly IAccommodationRepository accommodationRepository;
        private readonly ILocationRepository locationRepository;
        private readonly IAccommodationImageRepository accommodationImageRepository;

        public AccommodationService(IAccommodationRepository accommodationRepository, ILocationRepository locationRepository,IAccommodationImageRepository accommodationImageRepository)
        {
            this.accommodationRepository = accommodationRepository;
            this.locationRepository = locationRepository;
            this.accommodationImageRepository = accommodationImageRepository;
        }

        public List<AccommodationDTO> GetAllAccommodations()
        {
            return accommodationRepository.GetAll().Select(a => new AccommodationDTO(a)).ToList();
        }

        public AccommodationDTO GetAccommodationById(int id)
        {
            var accommodation = accommodationRepository.GetById(id);
            if (accommodation == null)
            {
                return null;
            }
            return new AccommodationDTO(accommodation);
        }

        public AccommodationDTO AddAccommodation(AccommodationDTO accommodation)
        {
            if (!accommodation.ToAccommodation().IsValid())
                throw new System.Exception("Accommodation is not valid");
            accommodationRepository.Save(accommodation.ToAccommodation());
            return accommodation;
        }

        public void DeleteAccommodation(AccommodationDTO accommodation)
        {
            accommodationRepository.Delete(accommodation.ToAccommodation());
        }

        public AccommodationDTO UpdateAccommodation(AccommodationDTO accommodation)
        {
            return new AccommodationDTO(accommodationRepository.Update(accommodation.ToAccommodation()));
        }

        public List<AccommodationDTO> GetAccommodationsByLocation(LocationDTO location)
        {
            return accommodationRepository
                       .GetByLocation(location.ToLocation())   
                       .Select(a => new AccommodationDTO(a))   
                       .ToList();
        }
        public bool RegisterAccommodation(AccommodationDTO accommodation)
        {
            var newAccommodation = accommodation.ToAccommodation();

            if (!newAccommodation.IsValid())
                return false;

            var savedLocation = locationRepository.Save(newAccommodation.GeoLocation);
            newAccommodation.GeoLocation = savedLocation;

            var savedAccommodation = accommodationRepository.Save(newAccommodation);

            foreach (var imageDto in accommodation.ImagePaths)
            {
                var image = new AccommodationImage
                {
                    Path = imageDto.Path,
                    AccommodationId = savedAccommodation.Id
                };

                accommodationImageRepository.Save(image);
            }

            return true;
        }
    }
}
