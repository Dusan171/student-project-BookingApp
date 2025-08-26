using BookingApp.Domain;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ImageRepository
    {
        private const string FilePath = "../../../Resources/Data/images.csv";

        private readonly Serializer<ImagePaths> _serializer;
        private List<ImagePaths> _images;

        public ImageRepository()
        {
            _serializer = new Serializer<ImagePaths>();
            _images = _serializer.FromCSV(FilePath);
        }

        public List<ImagePaths> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public ImagePaths Save(ImagePaths image)
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

        public void Delete(ImagePaths image)
        {
            _images = _serializer.FromCSV(FilePath);
            ImagePaths found = _images.Find(c => c.Id == image.Id);
            _images.Remove(found);
            _serializer.ToCSV(FilePath, _images);
        }

        public ImagePaths Update(ImagePaths image)
        {
            _images = _serializer.FromCSV(FilePath);
            ImagePaths current = _images.Find(c => c.Id == image.Id);
            int index = _images.IndexOf(current);
            _images.Remove(current);
            _images.Insert(index, image);
            _serializer.ToCSV(FilePath, _images);
            return image;
        }
    }
}
