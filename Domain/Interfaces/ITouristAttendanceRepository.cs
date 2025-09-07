using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITouristAttendanceRepository
    {
        List<TouristAttendance> GetAll();
        TouristAttendance? GetById(int id);
        List<TouristAttendance> GetByTourId(int tourId);
        List<TouristAttendance> GetByGuestId(int guestId);
        TouristAttendance Save(TouristAttendance attendance);
        TouristAttendance Add(TouristAttendance attendance);
        TouristAttendance Update(TouristAttendance attendance);
        void Delete(TouristAttendance attendance);
    }
}
