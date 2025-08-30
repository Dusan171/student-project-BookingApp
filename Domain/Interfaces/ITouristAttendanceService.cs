using BookingApp.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface ITouristAttendanceService
    {
        List<TouristAttendanceDTO> GetAll();
        TouristAttendanceDTO? GetById(int id);
        List<TouristAttendanceDTO> GetByTourId(int tourId);
        List<TouristAttendanceDTO> GetByGuestId(int guestId);
        TouristAttendanceDTO Create(TouristAttendanceDTO dto);
        TouristAttendanceDTO Update(TouristAttendanceDTO dto);
        void Delete(int id);
    }
}
