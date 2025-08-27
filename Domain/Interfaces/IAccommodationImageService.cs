using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationImageService
    {
        BitmapImage LoadImage(string relativePath);
        List<AccommodationImageDTO> GetAllImages();
        AccommodationImageDTO AddImage(AccommodationImageDTO image);
        void DeleteImage(AccommodationImageDTO image);
        AccommodationImageDTO UpdateImage(AccommodationImageDTO image);
        List<AccommodationImageDTO> GetImagesByAccommodation(AccommodationDTO accommodation);
    }
}
