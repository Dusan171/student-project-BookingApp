using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationImageService
    {
        List<AccommodationImage> GetAllImages();
        AccommodationImage AddImage(AccommodationImage image);
        void DeleteImage(AccommodationImage image);
        AccommodationImage UpdateImage(AccommodationImage image);
        List<AccommodationImage> GetImagesByAccommodation(Accommodation accommodation);
    }
}
