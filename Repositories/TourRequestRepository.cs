using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;

namespace BookingApp.Repositories
{
    public class TourRequestRepository : ITourRequestRepository
    {
        private const string FilePath = "../../../Resources/Data/tour_requests.csv";
        private readonly Serializer<TourRequest> _serializer;
        private List<TourRequest> _tourRequests;

        public TourRequestRepository()
        {
            _serializer = new Serializer<TourRequest>();
            _tourRequests = _serializer.FromCSV(FilePath);
        }

        public TourRequest GetById(int id)
        {
            return _tourRequests.FirstOrDefault(tr => tr.Id == id);
        }

        public List<TourRequest> GetAll()
        {
            return _tourRequests.ToList();
        }

        public TourRequest Save(TourRequest tourRequest)
        {
            tourRequest.Id = NextId();
            _tourRequests.Add(tourRequest);
            _serializer.ToCSV(FilePath, _tourRequests);
            return tourRequest;
        }

        public TourRequest Update(TourRequest tourRequest)
        {
            var existingRequest = GetById(tourRequest.Id);
            if (existingRequest != null)
            {
                existingRequest.TouristId = tourRequest.TouristId;
                existingRequest.City = tourRequest.City;
                existingRequest.Country = tourRequest.Country;
                existingRequest.Description = tourRequest.Description;
                existingRequest.Language = tourRequest.Language;
                existingRequest.NumberOfPeople = tourRequest.NumberOfPeople;
                existingRequest.DateFrom = tourRequest.DateFrom;
                existingRequest.DateTo = tourRequest.DateTo;
                existingRequest.Status = tourRequest.Status;
                existingRequest.AcceptedByGuideId = tourRequest.AcceptedByGuideId;
                existingRequest.AcceptedDate = tourRequest.AcceptedDate;
                existingRequest.ScheduledDate = tourRequest.ScheduledDate;

                _serializer.ToCSV(FilePath, _tourRequests);
                return existingRequest;
            }
            return null;
        }

        public void Delete(TourRequest tourRequest)
        {
            _tourRequests.RemoveAll(tr => tr.Id == tourRequest.Id);
            _serializer.ToCSV(FilePath, _tourRequests);
        }

        public List<TourRequest> GetByTouristId(int touristId)
        {
            return _tourRequests.Where(tr => tr.TouristId == touristId).ToList();
        }

        public List<TourRequest> GetPendingRequests()
        {
            return _tourRequests.Where(tr => tr.Status == TourRequestStatus.PENDING).ToList();
        }

        public List<TourRequest> GetRequestsByStatus(TourRequestStatus status)
        {
            return _tourRequests.Where(tr => tr.Status == status).ToList();
        }

        public List<TourRequest> GetExpiredRequests()
        {
            var twoDaysFromNow = DateTime.Now.AddDays(2);
            return _tourRequests.Where(tr => tr.Status == TourRequestStatus.PENDING &&
                                           tr.DateFrom <= twoDaysFromNow).ToList();
        }

        private int NextId()
        {
            return _tourRequests.Count > 0 ? _tourRequests.Max(tr => tr.Id) + 1 : 1;
        }
    }
}
