using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourRequestRepository
    {
        TourRequest GetById(int id);
        List<TourRequest> GetAll();
        TourRequest Save(TourRequest tourRequest);
        TourRequest Update(TourRequest tourRequest);
        void Delete(TourRequest tourRequest);
        List<TourRequest> GetByTouristId(int touristId);
        List<TourRequest> GetPendingRequests();
        List<TourRequest> GetRequestsByStatus(TourRequestStatus status);
        List<TourRequest> GetExpiredRequests();
    }
}
