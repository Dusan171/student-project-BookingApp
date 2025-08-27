using BookingApp.Domain;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class StartTourTimeRepository
    {
        private const string FilePath = "../../../Resources/Data/startTourTimes.csv";

        private readonly Serializer<StartTourTime> _serializer;
        private List<StartTourTime> _startTimes;

        public StartTourTimeRepository()
        {
            _serializer = new Serializer<StartTourTime>();
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
        }

        public List<StartTourTime> GetAll()
        {
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            return _startTimes;
        }

        public StartTourTime GetById(int id)
        {
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            return _startTimes.FirstOrDefault(t => t.Id == id);
        }

        public StartTourTime Save(StartTourTime startTime)
        {
            startTime.Id = NextId();
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            _startTimes.Add(startTime);
            _serializer.ToCSV(FilePath, _startTimes);
            return startTime;
        }

        public StartTourTime Update(StartTourTime startTime)
        {
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            var current = _startTimes.FirstOrDefault(t => t.Id == startTime.Id);
            if (current != null)
            {
                int index = _startTimes.IndexOf(current);
                _startTimes[index] = startTime;
                _serializer.ToCSV(FilePath, _startTimes);
            }
            return startTime;
        }

        public void Delete(StartTourTime startTime)
        {
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            var found = _startTimes.FirstOrDefault(t => t.Id == startTime.Id);
            if (found != null)
            {
                _startTimes.Remove(found);
                _serializer.ToCSV(FilePath, _startTimes);
            }
        }

        public int NextId()
        {
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            return _startTimes.Count == 0 ? 1 : _startTimes.Max(t => t.Id) + 1;
        }
    }
}
