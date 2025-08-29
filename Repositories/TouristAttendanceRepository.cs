using BookingApp.Domain;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Serializer;
using System.IO;
using BookingApp.Domain.Model;

namespace BookingApp.Repositories
{
    public class TouristAttendanceRepository
    {
        private const string FilePath = "../../../Resources/Data/touristAttendances.csv";

        private readonly Serializer<TouristAttendance> _serializer;
        private List<TouristAttendance> _attendances;

        public TouristAttendanceRepository()
        {
            _serializer = new Serializer<TouristAttendance>();
            _attendances = _serializer.FromCSV(FilePath) ?? new List<TouristAttendance>();
        }

        public List<TouristAttendance> GetAll()
        {
            _attendances = _serializer.FromCSV(FilePath) ?? new List<TouristAttendance>();
            return _attendances;
        }

        public TouristAttendance GetById(int id)
        {
            _attendances = _serializer.FromCSV(FilePath) ?? new List<TouristAttendance>();
            return _attendances.FirstOrDefault(a => a.Id == id);
        }

        public TouristAttendance Save(TouristAttendance attendance)
        {
            if (attendance.Id == 0)
            {
                attendance.Id = NextId();
            }

            _attendances = _serializer.FromCSV(FilePath) ?? new List<TouristAttendance>();
            _attendances.Add(attendance);
            _serializer.ToCSV(FilePath, _attendances);

            return attendance;
        }

        public int NextId()
        {
            _attendances = _serializer.FromCSV(FilePath) ?? new List<TouristAttendance>();
            if (_attendances.Count < 1)
                return 1;

            return _attendances.Max(a => a.Id) + 1;
        }

        public void Delete(TouristAttendance attendance)
        {
            _attendances = _serializer.FromCSV(FilePath) ?? new List<TouristAttendance>();
            var found = _attendances.FirstOrDefault(a => a.Id == attendance.Id);
            if (found != null)
            {
                _attendances.Remove(found);
                _serializer.ToCSV(FilePath, _attendances);
            }
        }

        public TouristAttendance Update(TouristAttendance attendance)
        {
            _attendances = _serializer.FromCSV(FilePath) ?? new List<TouristAttendance>();
            var current = _attendances.FirstOrDefault(a => a.Id == attendance.Id);
            if (current != null)
            {
                int index = _attendances.IndexOf(current);
                _attendances[index] = attendance;
                _serializer.ToCSV(FilePath, _attendances);
            }
            return attendance;
        }
       




    }
}
