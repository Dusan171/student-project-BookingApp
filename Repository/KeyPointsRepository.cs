using BookingApp.Model;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repository
{
    public class KeyPointRepository
    {
        private const string FilePath = "../../../Resources/Data/keypoints.csv";

        private readonly Serializer<KeyPoint> _serializer;
        private List<KeyPoint> _keyPoints;

        public KeyPointRepository()
        {
            _serializer = new Serializer<KeyPoint>();
            _keyPoints = _serializer.FromCSV(FilePath);
        }

        public List<KeyPoint> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public KeyPoint Save(KeyPoint keyPoint)
        {
            keyPoint.Id = NextId();
            _keyPoints = _serializer.FromCSV(FilePath);
            _keyPoints.Add(keyPoint);
            _serializer.ToCSV(FilePath, _keyPoints);
            return keyPoint;
        }

        public int NextId()
        {
            _keyPoints = _serializer.FromCSV(FilePath);
            if (_keyPoints.Count < 1)
            {
                return 1;
            }
            return _keyPoints.Max(c => c.Id) + 1;
        }

        public void Delete(KeyPoint keyPoint)
        {
            _keyPoints = _serializer.FromCSV(FilePath);
            KeyPoint found = _keyPoints.Find(c => c.Id == keyPoint.Id);
            _keyPoints.Remove(found);
            _serializer.ToCSV(FilePath, _keyPoints);
        }

        public KeyPoint Update(KeyPoint keyPoint)
        {
            _keyPoints = _serializer.FromCSV(FilePath);
            KeyPoint current = _keyPoints.Find(c => c.Id == keyPoint.Id);
            int index = _keyPoints.IndexOf(current);
            _keyPoints.Remove(current);
            _keyPoints.Insert(index, keyPoint);
            _serializer.ToCSV(FilePath, _keyPoints);
            return keyPoint;
        }
    }
}
