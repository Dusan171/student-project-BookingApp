using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class TouristAttendanceRepository : ITouristAttendanceRepository
    {
        private const string FilePath = "../../../Resources/Data/touristAttendances.csv";
        private readonly Serializer<TouristAttendance> _serializer;
        private List<TouristAttendance> _touristAttendances;

        public TouristAttendanceRepository()
        {
            _serializer = new Serializer<TouristAttendance>();
            _touristAttendances = _serializer.FromCSV(FilePath);
        }

        public List<TouristAttendance> GetAll()
        {
            return _touristAttendances;
        }

        public TouristAttendance? GetById(int id)
        {
            return _touristAttendances.FirstOrDefault(ta => ta.Id == id);
        }

        public List<TouristAttendance> GetByTourId(int tourId)
        {
            return _touristAttendances.Where(ta => ta.TourId == tourId).ToList();
        }

        public List<TouristAttendance> GetByGuestId(int guestId)
        {
            return _touristAttendances.Where(ta => ta.GuestId == guestId).ToList();
        }

        public TouristAttendance Add(TouristAttendance attendance)
        {
            attendance.Id = NextId();
            _touristAttendances.Add(attendance);
            _serializer.ToCSV(FilePath, _touristAttendances);
            return attendance;
        }

        public TouristAttendance Update(TouristAttendance attendance)
        {
            var existingAttendance = GetById(attendance.Id);
            if (existingAttendance != null)
            {
                var index = _touristAttendances.IndexOf(existingAttendance);
                _touristAttendances[index] = attendance;
                _serializer.ToCSV(FilePath, _touristAttendances);
            }
            return attendance;
        }

        public void Delete(TouristAttendance attendance)
        {
            _touristAttendances.Remove(attendance);
            _serializer.ToCSV(FilePath, _touristAttendances);
        }

        public TouristAttendance Save(TouristAttendance attendance)
        {
            if (attendance.Id == 0)
            {
                return Add(attendance);
            }
            else
            {
                return Update(attendance);
            }
        }

        private int NextId()
        {
            return _touristAttendances.Count > 0 ? _touristAttendances.Max(ta => ta.Id) + 1 : 1;
        }
    }
}