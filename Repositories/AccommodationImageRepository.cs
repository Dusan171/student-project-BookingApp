using BookingApp.Domain;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BookingApp.Repositories
{
    public class AccommodationImageRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationImage.csv";

        private readonly Serializer<AccommodationImage> _serializer;

        private List<AccommodationImage> _images;

        public AccommodationImageRepository()
        {
            _serializer = new Serializer<AccommodationImage>();
            _images = _serializer.FromCSV(FilePath);
        }

        public List<AccommodationImage> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public AccommodationImage Save(AccommodationImage image)
        {
            image.Id = NextId();
            _images = _serializer.FromCSV(FilePath);
            _images.Add(image);
            _serializer.ToCSV(FilePath, _images);
            return image;
        }

        public int NextId()
        {
            _images = _serializer.FromCSV(FilePath);
            if (_images.Count < 1)
            {
                return 1;
            }
            return _images.Max(c => c.Id) + 1;
        }

        public void Delete(AccommodationImage image)
        {
            _images = _serializer.FromCSV(FilePath);
            AccommodationImage founded = _images.Find(c => c.Id == image.Id);
            _images.Remove(founded);
            _serializer.ToCSV(FilePath, _images);
        }

        public AccommodationImage Update(AccommodationImage image)
        {
            _images = _serializer.FromCSV(FilePath);
            AccommodationImage current = _images.Find(c => c.Id == image.Id);
            int index = _images.IndexOf(current);
            _images.Remove(current);
            _images.Insert(index, image);       // keep ascending order of ids in file 
            _serializer.ToCSV(FilePath, _images);
            return image;
        }

        public List<AccommodationImage> GetByAccommodation(Accommodation accommodation)
        {
            _images = _serializer.FromCSV(FilePath);
            return _images.FindAll(c => c.AccommodationId == accommodation.Id);
        }


    }
}
