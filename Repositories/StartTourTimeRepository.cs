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
        private List<StartTourTime> _startTourTimes;

        public StartTourTimeRepository()
        {
            _serializer = new Serializer<StartTourTime>();
            _startTourTimes = _serializer.FromCSV(FilePath);
        }

        public List<StartTourTime> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public StartTourTime GetById(int id)
        {
            _startTourTimes = _serializer.FromCSV(FilePath);
            return _startTourTimes.FirstOrDefault(st => st.Id == id);
        }


        public StartTourTime Save(StartTourTime startTourTime)
        {
            startTourTime.Id = NextId();
            _startTourTimes = _serializer.FromCSV(FilePath);
            _startTourTimes.Add(startTourTime);
            _serializer.ToCSV(FilePath, _startTourTimes);
            return startTourTime;
        }

        public int NextId()
        {
            _startTourTimes = _serializer.FromCSV(FilePath);
            if (_startTourTimes.Count < 1)
            {
                return 1;
            }
            return _startTourTimes.Max(c => c.Id) + 1;
        }

        public void Delete(StartTourTime startTourTime)
        {
            _startTourTimes = _serializer.FromCSV(FilePath);
            StartTourTime found = _startTourTimes.Find(c => c.Id == startTourTime.Id);
            _startTourTimes.Remove(found);
            _serializer.ToCSV(FilePath, _startTourTimes);
        }

        public StartTourTime Update(StartTourTime startTourTime)
        {
            _startTourTimes = _serializer.FromCSV(FilePath);
            StartTourTime current = _startTourTimes.Find(c => c.Id == startTourTime.Id);
            int index = _startTourTimes.IndexOf(current);
            _startTourTimes.Remove(current);
            _startTourTimes.Insert(index, startTourTime);
            _serializer.ToCSV(FilePath, _startTourTimes);
            return startTourTime;
        }
    }
}
