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
        private List<StartTourTime> _startTourTimes;

        public StartTourTimeRepository()
        {
            _serializer = new Serializer<StartTourTime>();
            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
        }

        public List<StartTourTime> GetAll()
        {
            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            return _startTourTimes.ToList();
        }

        public StartTourTime? GetById(int id)
        {
            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            return _startTourTimes.FirstOrDefault(st => st.Id == id);
        }

        public StartTourTime Save(StartTourTime startTourTime)
        {
            if (startTourTime == null)
                throw new ArgumentNullException(nameof(startTourTime));

            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();

            if (startTourTime.Id == 0)
            {
                startTourTime.Id = GetNextId();
            }

            _startTourTimes.Add(startTourTime);
            SaveAll();
            return startTourTime;
        }

        public StartTourTime Update(StartTourTime startTourTime)
        {
            if (startTourTime == null)
                throw new ArgumentNullException(nameof(startTourTime));

            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            var current = _startTourTimes.Find(c => c.Id == startTourTime.Id);

            if (current != null)
            {
                int index = _startTourTimes.IndexOf(current);
                _startTourTimes.Remove(current);
                _startTourTimes.Insert(index, startTourTime);
                SaveAll();
            }

            return startTourTime;
        }

        public void Delete(StartTourTime startTourTime)
        {
            if (startTourTime == null)
                throw new ArgumentNullException(nameof(startTourTime));

            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            var found = _startTourTimes.Find(c => c.Id == startTourTime.Id);

            if (found != null)
            {
                _startTourTimes.Remove(found);
                SaveAll();
            }
        }

        public void SaveAll()
        {
            _serializer.ToCSV(FilePath, _startTourTimes);
        }

        public int GetNextId()
        {
            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            return _startTourTimes.Count == 0 ? 1 : _startTourTimes.Max(c => c.Id) + 1;
        }

        public int NextId()
        {
            return GetNextId();
        }

        public List<StartTourTime> GetByTourId(int tourId)
        {
            // Ova implementacija zavisi od toga kako se čuva veza između Tour i StartTourTime
            // Možda trebate dodatnu kolonu u CSV ili poseban mapping fajl
            _startTourTimes = _serializer.FromCSV(FilePath) ?? new List<StartTourTime>();
            return _startTourTimes.ToList(); // Placeholder implementacija
        }
    }
}