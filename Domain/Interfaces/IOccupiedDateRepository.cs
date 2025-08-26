using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IOccupiedDateRepository
    {
        List<OccupiedDate> GetAll();
        List<OccupiedDate> GetByAccommodationId(int accommodationId);
        void Save(List<OccupiedDate> newDates);
    }
}
