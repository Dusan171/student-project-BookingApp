using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IComplexTourRequestRepository
    {
        ComplexTourRequest GetById(int id);
        List<ComplexTourRequest> GetAll();
        ComplexTourRequest Save(ComplexTourRequest request);
        ComplexTourRequest Update(ComplexTourRequest request);
        void Delete(ComplexTourRequest request);
        List<ComplexTourRequest> GetByTouristId(int touristId);
        List<ComplexTourRequest> GetPendingRequests();
        List<ComplexTourRequest> GetExpiredRequests();
    }
}