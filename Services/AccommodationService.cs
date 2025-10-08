using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;
using BookingApp.Repositories;
using BookingApp.Domain.Interfaces.ServiceInterfaces;

namespace BookingApp.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IAccommodationImageRepository _accommodationImageRepository;
        private readonly IAccommodationValidationService _accommodationValidationService;
        public AccommodationService(IAccommodationRepository accommodationRepository, ILocationRepository locationRepository, IAccommodationImageRepository accommodationImageRepository, IAccommodationValidationService accommodationValidationService)
        {
            _accommodationRepository = accommodationRepository;
            _locationRepository = locationRepository;
            _accommodationImageRepository = accommodationImageRepository;
            _accommodationValidationService = accommodationValidationService;
        }
        public List<AccommodationDTO> GetAllAccommodations()
        {
            return _accommodationRepository.GetAll().Select(a => new AccommodationDTO(a)).ToList();
        }
        public AccommodationDTO GetAccommodationById(int id)
        {
            var accommodation = _accommodationRepository.GetById(id);
            if (accommodation == null)
            {
                return null;
            }
            return new AccommodationDTO(accommodation);
        }
        public AccommodationDTO AddAccommodation(AccommodationDTO accommodation)
        {
            if (!_accommodationValidationService.IsAccommodationValid(accommodation, out string errorMessage))
                throw new System.Exception("Accommodation is not valid");
            _accommodationRepository.Save(accommodation.ToAccommodation());
            return accommodation;
        }
        public void DeleteAccommodation(AccommodationDTO accommodation)
        {
            _accommodationRepository.Delete(accommodation.ToAccommodation());
        }
        public AccommodationDTO UpdateAccommodation(AccommodationDTO accommodation)
        {
            return new AccommodationDTO(_accommodationRepository.Update(accommodation.ToAccommodation()));
        }
        public List<AccommodationDTO> GetAccommodationsByLocation(LocationDTO location)
        {
            return _accommodationRepository
                       .GetByLocation(location.ToLocation())
                       .Select(a => new AccommodationDTO(a))
                       .ToList();
        }
        public bool RegisterAccommodation(AccommodationDTO accommodationDto)
        {
            if (!_accommodationValidationService.IsAccommodationValid(accommodationDto, out string errorMessage))
            {
                return false;
            }
            var newAccommodation = accommodationDto.ToAccommodation();

            SaveAccommodation(newAccommodation);
            SaveAccommodationImages(accommodationDto, newAccommodation.Id);

            return true;
        }
        private void SaveAccommodation(Accommodation accommodation)
        {
            var savedLocation = _locationRepository.Save(accommodation.GeoLocation);
            accommodation.GeoLocation = savedLocation;
            _accommodationRepository.Save(accommodation);
        }
        private void SaveAccommodationImages(AccommodationDTO accommodationDto, int accommodationId)
        {
            foreach (var imageDto in accommodationDto.ImagePaths)
            {
                var image = new AccommodationImage
                {
                    Path = imageDto.Path,
                    AccommodationId = accommodationId
                };
                _accommodationImageRepository.Save(image);
            }
        }
        public List<AccommodationDTO> GetAccommodationsByOwnerId(int ownerId)
        {
            return _accommodationRepository.GetAll()
                .Where(a => a.OwnerId == ownerId)
                .Select(a => new AccommodationDTO(a))
                .ToList();
        }

        public int GetActiveAccommodationsCount(int ownerId)
        {
            return _accommodationRepository.GetAll()
                .Count(a => a.OwnerId == ownerId);
        }

        public int GetTotalAccommodationsCount(int ownerId)
        {
            return _accommodationRepository.GetAll()
                .Count(a => a.OwnerId == ownerId);
        }
        public List<Accommodation> GetByOwnerId(int ownerId)
        {
            return _accommodationRepository.GetByOwnerId(ownerId);
        }

        public string GetLocationString(int locationId)
        {
            
            var location = _locationRepository.GetById(locationId);

            if (location != null)
            {
                return $"{location.City}, {location.Country}";
            }

            return "Unknown Location";
        }
    }
}
