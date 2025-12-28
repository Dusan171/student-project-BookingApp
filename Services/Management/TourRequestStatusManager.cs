using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;

namespace BookingApp.Services.Management
{
    public class TourRequestStatusManager
    {
        private readonly ITourRequestRepository _requestRepository;
        private readonly INotificationService _notificationService;

        public TourRequestStatusManager(ITourRequestRepository requestRepository, INotificationService notificationService)
        {
            _requestRepository = requestRepository;
            _notificationService = notificationService;
        }

        public void AcceptRequest(int requestId, int guideId, DateTime scheduledDate)
        {
            var request = _requestRepository.GetById(requestId);
            if (request != null && CanAcceptRequest(request))
            {
                UpdateRequestAsAccepted(request, guideId, scheduledDate);
                _requestRepository.Update(request);
                // TODO: Send notification
            }
        }

        public void MarkRequestAsInvalid(int requestId)
        {
            var request = _requestRepository.GetById(requestId);
            if (request != null)
            {
                request.Status = TourRequestStatus.INVALID;
                _requestRepository.Update(request);
            }
        }

        public void CheckAndMarkExpiredRequests()
        {
            var expiredRequests = _requestRepository.GetExpiredRequests();
            foreach (var request in expiredRequests)
            {
                request.Status = TourRequestStatus.INVALID;
                _requestRepository.Update(request);
            }
        }

        public bool CanAcceptRequest(int requestId)
        {
            var request = _requestRepository.GetById(requestId);
            return CanAcceptRequest(request);
        }

        private bool CanAcceptRequest(TourRequest request)
        {
            return request != null && request.Status == TourRequestStatus.PENDING && request.IsValid;
        }

        private void UpdateRequestAsAccepted(TourRequest request, int guideId, DateTime scheduledDate)
        {
            request.Status = TourRequestStatus.ACCEPTED;
            request.AcceptedByGuideId = guideId;
            request.AcceptedDate = DateTime.Now;
            request.ScheduledDate = scheduledDate;
        }
    }
}