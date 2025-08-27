using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;
using System.Windows.Media.Imaging;
using System.IO;

namespace BookingApp.Services
{
    public class AccommodationImageService : IAccommodationImageService
    {
        private readonly IAccommodationImageRepository _repository;

        public AccommodationImageService(IAccommodationImageRepository repository)
        {
            _repository = repository;
            _baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images");

        }

        public List<AccommodationImageDTO> GetAllImages()
        {
            return _repository.GetAll()
                      .Select(image => new AccommodationImageDTO(image))
                      .ToList();
        }

        public AccommodationImageDTO AddImage(AccommodationImageDTO image)
        {
            if (image == null || string.IsNullOrWhiteSpace(image.Path))
                throw new System.Exception("Image is not valid");

            return new AccommodationImageDTO(_repository.Save(image.ToAccommodationImage()));
        }

        public void DeleteImage(AccommodationImageDTO image)
        {
            _repository.Delete(image.ToAccommodationImage());
        }

        public AccommodationImageDTO UpdateImage(AccommodationImageDTO image)
        {
            return new AccommodationImageDTO( _repository.Update(image.ToAccommodationImage()));
        }

        public List<AccommodationImageDTO> GetImagesByAccommodation(AccommodationDTO accommodation)
        {
            return _repository.GetByAccommodation(accommodation.ToAccommodation())
                       .Select(image => new AccommodationImageDTO(image))
                       .ToList();
        }
        private readonly string _baseDir;

        public BitmapImage LoadImage(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(_baseDir, relativePath);
                if (!File.Exists(fullPath))
                    return null;

                return new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            }
            catch
            {
                return null;
            }
        
        }
    }
}
