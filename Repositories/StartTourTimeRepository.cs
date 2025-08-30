using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class StartTourTimeRepository : IStartTourTimeRepository
    {
        private const string FilePath = "../../../Resources/Data/startTourTimes.csv";
        private readonly Serializer<StartTourTime> _serializer;
        private List<StartTourTime> _startTimes;

        public StartTourTimeRepository()
        {
            _serializer = new Serializer<StartTourTime>();
            _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
        }

        public void SaveAll() => _serializer.ToCSV(FilePath, _startTimes);

        private void Reload() => _startTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();

        public List<StartTourTime> GetAll() => _startTimes.ToList();

        public StartTourTime? GetById(int id) => _startTimes.FirstOrDefault(st => st.Id == id);

        public List<StartTourTime> GetByTourId(int tourId)
        {
            // Ovde treba filtrirati po TourId ako postoji kolona u CSV
            return _startTimes.Where(st => st.TourId == tourId).ToList();
        }

        public StartTourTime Save(StartTourTime startTourTime)
        {
            var existing = _startTimes.FirstOrDefault(st => st.Id == startTourTime.Id);
            if (existing == null)
                return Add(startTourTime);
            else
                return Update(startTourTime);
        }
        public StartTourTime Add(StartTourTime startTime)
        {
            if (startTime == null) throw new ArgumentNullException(nameof(startTime));

            startTime.Id = GetNextId();
            _startTimes.Add(startTime);
            SaveAll();
            return startTime;
        }

        public StartTourTime Update(StartTourTime startTime)
        {
            if (startTime == null) throw new ArgumentNullException(nameof(startTime));

            var index = _startTimes.FindIndex(st => st.Id == startTime.Id);
            if (index >= 0)
            {
                _startTimes[index] = startTime;
                SaveAll();
            }

            return startTime;
        }

        public void Delete(StartTourTime startTime)
        {
            if (startTime == null) throw new ArgumentNullException(nameof(startTime));

            var existing = _startTimes.FirstOrDefault(st => st.Id == startTime.Id);
            if (existing != null)
            {
                _startTimes.Remove(existing);
                SaveAll();
            }
        }

        public int NextId() => GetNextId();

        public int GetNextId() => _startTimes.Count == 0 ? 1 : _startTimes.Max(st => st.Id) + 1;
    }
}
