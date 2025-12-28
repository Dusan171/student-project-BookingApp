using System;
using System.Collections.Generic;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using BookingApp.Services.Validation;
using BookingApp.Services.Management;
using BookingApp.Services.Enrichment;
using BookingApp.Services.Factory;

namespace BookingApp.Services
{
    public class TourRequestService : ITourRequestService
    {
        private readonly ITourRequestRepository _requestRepository;
        private readonly TourRequestValidator _validator;
        private readonly TourRequestParticipantManager _participantManager;
        private readonly TourRequestEnricher _enricher;
        private readonly TourRequestStatusManager _statusManager;

        public TourRequestService(ITourRequestRepository requestRepository, ITourRequestParticipantRepository participantRepository, 
            IUserRepository userRepository, INotificationService notificationService)
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            (_validator, _participantManager, _enricher, _statusManager) = 
                TourRequestServiceFactory.CreateDependencies(requestRepository, participantRepository, userRepository, notificationService);
        }

        // CRUD Operations
        public List<TourRequestDTO> GetAllRequests() => _enricher.EnrichAndConvertToDTO(_requestRepository.GetAll());
        public TourRequestDTO GetRequestById(int id) => _requestRepository.GetById(id) == null ? null : _enricher.EnrichAndConvertToDTO(_requestRepository.GetById(id));
        public TourRequestDTO UpdateRequest(TourRequestDTO requestDTO) => ValidateAndEnrich(requestDTO, r => _requestRepository.Update(r.ToTourRequest()));
        public void DeleteRequest(int id) => ExecuteIfExists(id, request => { _participantManager.DeleteAllParticipants(id); _requestRepository.Delete(request); });

        public TourRequestDTO CreateRequest(TourRequestDTO requestDTO)
        {
            ValidateRequestDTO(requestDTO);
            var saved = _requestRepository.Save(requestDTO.ToTourRequest());
            _participantManager.SaveParticipants(saved.Id, requestDTO.Participants);
            return _enricher.EnrichAndConvertToDTO(saved);
        }

        // Query Operations
        public List<TourRequestDTO> GetRequestsByTourist(int touristId) => _enricher.EnrichAndConvertToDTO(_requestRepository.GetByTouristId(touristId));
        public List<TourRequestDTO> GetPendingRequests() => _enricher.EnrichAndConvertToDTO(_requestRepository.GetPendingRequests());
        public List<TourRequestDTO> GetRequestsByStatus(TourRequestStatus status) => _enricher.EnrichAndConvertToDTO(_requestRepository.GetRequestsByStatus(status));

        // Status Management
        public void AcceptRequest(int requestId, int guideId, DateTime scheduledDate) => _statusManager.AcceptRequest(requestId, guideId, scheduledDate);
        public void MarkRequestAsInvalid(int requestId) => _statusManager.MarkRequestAsInvalid(requestId);
        public void CheckAndMarkExpiredRequests() => _statusManager.CheckAndMarkExpiredRequests();
        public bool CanAcceptRequest(int requestId) => _statusManager.CanAcceptRequest(requestId);

        // Participant Management
        public TourRequestParticipantDTO AddParticipant(int requestId, TourRequestParticipantDTO participantDTO) => _participantManager.AddParticipant(requestId, participantDTO);
        public void RemoveParticipant(int participantId) => _participantManager.RemoveParticipant(participantId);
        public List<TourRequestParticipantDTO> GetParticipantsByRequest(int requestId) => _participantManager.GetParticipantsByRequest(requestId);

        // Helper Methods
        private void ValidateRequestDTO(TourRequestDTO requestDTO)
        {
            if (requestDTO == null) throw new ArgumentNullException(nameof(requestDTO));
            if (!_validator.ValidateRequest(requestDTO)) throw new ArgumentException("Invalid tour request data");
        }

        private TourRequestDTO ValidateAndEnrich(TourRequestDTO requestDTO, Func<TourRequestDTO, TourRequest> operation)
        {
            if (requestDTO == null) throw new ArgumentNullException(nameof(requestDTO));
            return _enricher.EnrichAndConvertToDTO(operation(requestDTO));
        }

        private void ExecuteIfExists(int id, Action<TourRequest> action)
        {
            var request = _requestRepository.GetById(id);
            if (request != null) action(request);
        }
    }
}
