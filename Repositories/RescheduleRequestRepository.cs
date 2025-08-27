using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;
using BookingApp.Serializer;
using BookingApp.Domain.Interfaces;
using System.IO;


namespace BookingApp.Repositories
{
    public class RescheduleRequestRepository : IRescheduleRequestRepository
    {
        private const string FilePath = "../../../Resources/Data/rescheduleRequests.csv";
        private readonly Serializer<RescheduleRequest> _serializer;
        private List<RescheduleRequest> _requests;

        public RescheduleRequestRepository()
        {
            _serializer = new Serializer<RescheduleRequest>();
            _requests = _serializer.FromCSV(FilePath);
        }
        public List<RescheduleRequest> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }
        public RescheduleRequest Save(RescheduleRequest request)
        {
            _requests = GetAll();
            request.Id = NextId();
            _requests.Add(request);
            _serializer.ToCSV(FilePath, _requests);
            return request;
        }
        public int NextId()
        {
            _requests = GetAll();
            return _requests.Any() ? _requests.Max(r => r.Id) + 1 : 1;
        }
        

        public RescheduleRequest GetByReservationId(int reservationId)
        {
            return GetAll().FirstOrDefault(r => r.ReservationId == reservationId);
        }
       
        public bool HasPendingRequest(int reservationId)
        {
            var request = GetByReservationId(reservationId);
            return request != null && request.Status == RequestStatus.Pending;

        }
        public void SaveAll(List<RescheduleRequest> requests)
        {
            _serializer.ToCSV(FilePath, requests);
        }
    }
}
