using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class TourRequestService : ITourRequestService
    {
        private readonly ITourRequestRepository _requestRepository;
        private readonly ITourRequestParticipantRepository _participantRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public TourRequestService(ITourRequestRepository requestRepository,
                                ITourRequestParticipantRepository participantRepository,
                                IUserRepository userRepository,
                                INotificationService notificationService)
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public List<TourRequestDTO> GetAllRequests()
        {
            var requests = _requestRepository.GetAll();
            return EnrichRequestsAndConvertToDTO(requests);
        }

        public TourRequestDTO GetRequestById(int id)
        {
            var request = _requestRepository.GetById(id);
            if (request == null) return null;

            return EnrichRequestAndConvertToDTO(request);
        }

        public TourRequestDTO CreateRequest(TourRequestDTO requestDTO)
        {
            if (requestDTO == null)
                throw new ArgumentNullException(nameof(requestDTO));

            if (!ValidateRequest(requestDTO))
                throw new ArgumentException("Invalid tour request data");

            var request = requestDTO.ToTourRequest();
            var savedRequest = _requestRepository.Save(request);

            // Sačuvaj učesnike
            foreach (var participantDTO in requestDTO.Participants)
            {
                participantDTO.TourRequestId = savedRequest.Id;
                var participant = participantDTO.ToTourRequestParticipant();
                _participantRepository.Save(participant);
            }

            return EnrichRequestAndConvertToDTO(savedRequest);
        }

        public TourRequestDTO UpdateRequest(TourRequestDTO requestDTO)
        {
            if (requestDTO == null)
                throw new ArgumentNullException(nameof(requestDTO));

            var request = requestDTO.ToTourRequest();
            var updatedRequest = _requestRepository.Update(request);

            return EnrichRequestAndConvertToDTO(updatedRequest);
        }

        public void DeleteRequest(int id)
        {
            var request = _requestRepository.GetById(id);
            if (request != null)
            {
                // Obriši sve učesnike
                _participantRepository.DeleteByTourRequestId(id);

                // Obriši zahtev
                _requestRepository.Delete(request);
            }
        }

        public List<TourRequestDTO> GetRequestsByTourist(int touristId)
        {
            var requests = _requestRepository.GetByTouristId(touristId);
            return EnrichRequestsAndConvertToDTO(requests);
        }

        public List<TourRequestDTO> GetPendingRequests()
        {
            var requests = _requestRepository.GetPendingRequests();
            return EnrichRequestsAndConvertToDTO(requests);
        }

        public List<TourRequestDTO> GetRequestsByStatus(TourRequestStatus status)
        {
            var requests = _requestRepository.GetRequestsByStatus(status);
            return EnrichRequestsAndConvertToDTO(requests);
        }

        public void AcceptRequest(int requestId, int guideId, DateTime scheduledDate)
        {
            var request = _requestRepository.GetById(requestId);
            if (request != null && request.Status == TourRequestStatus.PENDING)
            {
                request.Status = TourRequestStatus.ACCEPTED;
                request.AcceptedByGuideId = guideId;
                request.AcceptedDate = DateTime.Now;
                request.ScheduledDate = scheduledDate;

                _requestRepository.Update(request);

                // Pošalji notifikaciju turistu
                // Ovde bi trebalo kreirati notifikaciju, ali možda treba nova vrsta
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
            return request != null && request.Status == TourRequestStatus.PENDING && request.IsValid;
        }

        public TourRequestParticipantDTO AddParticipant(int requestId, TourRequestParticipantDTO participantDTO)
        {
            if (participantDTO == null)
                throw new ArgumentNullException(nameof(participantDTO));

            participantDTO.TourRequestId = requestId;
            var participant = participantDTO.ToTourRequestParticipant();
            var savedParticipant = _participantRepository.Save(participant);

            return TourRequestParticipantDTO.FromDomain(savedParticipant);
        }

        public void RemoveParticipant(int participantId)
        {
            var participant = _participantRepository.GetById(participantId);
            if (participant != null)
            {
                _participantRepository.Delete(participant);
            }
        }

        public List<TourRequestParticipantDTO> GetParticipantsByRequest(int requestId)
        {
            var participants = _participantRepository.GetByTourRequestId(requestId);
            return participants.Select(p => TourRequestParticipantDTO.FromDomain(p)).ToList();
        }

        private bool ValidateRequest(TourRequestDTO requestDTO)
        {
            if (string.IsNullOrWhiteSpace(requestDTO.City) ||
                string.IsNullOrWhiteSpace(requestDTO.Country) ||
                string.IsNullOrWhiteSpace(requestDTO.Language) ||
                requestDTO.NumberOfPeople <= 0 ||
                requestDTO.DateFrom <= DateTime.Now.AddDays(1) ||
                requestDTO.DateTo <= requestDTO.DateFrom)
            {
                return false;
            }

            return true;
        }

        private List<TourRequestDTO> EnrichRequestsAndConvertToDTO(List<TourRequest> requests)
        {
            var dtos = new List<TourRequestDTO>();
            foreach (var request in requests)
            {
                dtos.Add(EnrichRequestAndConvertToDTO(request));
            }
            return dtos;
        }

        private TourRequestDTO EnrichRequestAndConvertToDTO(TourRequest request)
        {
            // Učitaj učesnike
            request.Participants = _participantRepository.GetByTourRequestId(request.Id);

            // Učitaj turista
            if (request.TouristId > 0)
            {
                request.Tourist = _userRepository.GetById(request.TouristId);
            }

            // Učitaj vodiča ako je prihvaćen
            if (request.AcceptedByGuideId.HasValue)
            {
                request.AcceptedGuide = _userRepository.GetById(request.AcceptedByGuideId.Value);
            }

            return TourRequestDTO.FromDomain(request);
        }
    }
}
