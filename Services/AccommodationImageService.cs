using BookingApp.Domain.Interfaces;
using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class AccommodationImageService : IAccommodationImageService
    {
        private readonly IAccommodationImageRepository _repository;

        public AccommodationImageService(IAccommodationImageRepository repository)
        {
            _repository = repository;
        }

        public List<AccommodationImage> GetAllImages()
        {
            return _repository.GetAll();
        }

        public AccommodationImage AddImage(AccommodationImage image)
        {
            if (image == null || string.IsNullOrWhiteSpace(image.Path))
                throw new System.Exception("Image is not valid");

            return _repository.Save(image);
        }

        public void DeleteImage(AccommodationImage image)
        {
            _repository.Delete(image);
        }

        public AccommodationImage UpdateImage(AccommodationImage image)
        {
            return _repository.Update(image);
        }

        public List<AccommodationImage> GetImagesByAccommodation(Accommodation accommodation)
        {
            return _repository.GetByAccommodation(accommodation);
        }
    }
}
