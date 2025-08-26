using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IRescheduleRequestRepository
    {
        List<RescheduleRequest> GetAll();
        RescheduleRequest Save(RescheduleRequest request);
        int NextId();
        RescheduleRequest GetByReservationId(int reservationId);
        bool HasPendingRequest(int reservationId);
    }
}
