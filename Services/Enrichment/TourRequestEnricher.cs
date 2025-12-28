using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Services.Enrichment
{
    public class TourRequestEnricher
    {
        private readonly ITourRequestParticipantRepository _participantRepository;
        private readonly IUserRepository _userRepository;

        public TourRequestEnricher(ITourRequestParticipantRepository participantRepository, IUserRepository userRepository)
        {
            _participantRepository = participantRepository;
            _userRepository = userRepository;
        }

        public List<TourRequestDTO> EnrichAndConvertToDTO(List<TourRequest> requests)
        {
            var dtos = new List<TourRequestDTO>();
            foreach (var request in requests)
            {
                dtos.Add(EnrichAndConvertToDTO(request));
            }
            return dtos;
        }

        public TourRequestDTO EnrichAndConvertToDTO(TourRequest request)
        {
            EnrichRequest(request);
            return TourRequestDTO.FromDomain(request);
        }

        private void EnrichRequest(TourRequest request)
        {
            LoadParticipants(request);
            LoadTourist(request);
            LoadAcceptedGuide(request);
        }

        private void LoadParticipants(TourRequest request)
        {
            request.Participants = _participantRepository.GetByTourRequestId(request.Id);
        }

        private void LoadTourist(TourRequest request)
        {
            if (request.TouristId > 0)
            {
                request.Tourist = _userRepository.GetById(request.TouristId);
            }
        }

        private void LoadAcceptedGuide(TourRequest request)
        {
            if (request.AcceptedByGuideId.HasValue)
            {
                request.AcceptedGuide = _userRepository.GetById(request.AcceptedByGuideId.Value);
            }
        }
    }
}