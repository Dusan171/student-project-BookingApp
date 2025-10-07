// ComplexTourRequestRepository.cs
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ComplexTourRequestRepository : IComplexTourRequestRepository
    {
        private const string FilePath = "../../../Resources/Data/complex_tour_requests.csv";
        private readonly Serializer<ComplexTourRequest> _serializer;
        private List<ComplexTourRequest> _complexTourRequests;

        public ComplexTourRequestRepository()
        {
            _serializer = new Serializer<ComplexTourRequest>();
            _complexTourRequests = _serializer.FromCSV(FilePath);
        }

        public ComplexTourRequest GetById(int id)
        {
            return _complexTourRequests.FirstOrDefault(ctr => ctr.Id == id);
        }

        public List<ComplexTourRequest> GetAll()
        {
            return _complexTourRequests.ToList();
        }

        public ComplexTourRequest Save(ComplexTourRequest request)
        {
            request.Id = NextId();
            request.CreatedAt = DateTime.Now;
            _complexTourRequests.Add(request);
            _serializer.ToCSV(FilePath, _complexTourRequests);
            return request;
        }

        public ComplexTourRequest Update(ComplexTourRequest request)
        {
            var existingRequest = GetById(request.Id);
            if (existingRequest != null)
            {
                existingRequest.TouristId = request.TouristId;
                existingRequest.CreatedAt = request.CreatedAt;
                existingRequest.Status = request.Status;
                existingRequest.InvalidationDeadline = request.InvalidationDeadline;

                _serializer.ToCSV(FilePath, _complexTourRequests);
                return existingRequest;
            }
            return null;
        }

        public void Delete(ComplexTourRequest request)
        {
            _complexTourRequests.RemoveAll(ctr => ctr.Id == request.Id);
            _serializer.ToCSV(FilePath, _complexTourRequests);
        }

        public List<ComplexTourRequest> GetByTouristId(int touristId)
        {
            return _complexTourRequests.Where(ctr => ctr.TouristId == touristId)
                                      .OrderByDescending(ctr => ctr.CreatedAt)
                                      .ToList();
        }

        public List<ComplexTourRequest> GetPendingRequests()
        {
            return _complexTourRequests.Where(ctr => ctr.Status == ComplexTourRequestStatus.PENDING)
                                      .ToList();
        }

        public List<ComplexTourRequest> GetExpiredRequests()
        {
            return _complexTourRequests
                .Where(r => r.Status == ComplexTourRequestStatus.PENDING &&
                           DateTime.Now > r.InvalidationDeadline)
                .ToList();
        }

        private int NextId()
        {
            return _complexTourRequests.Count > 0 ? _complexTourRequests.Max(ctr => ctr.Id) + 1 : 1;
        }
    }
}