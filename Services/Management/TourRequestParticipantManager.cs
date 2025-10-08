using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Services.Management
{
    public class TourRequestParticipantManager
    {
        private readonly ITourRequestParticipantRepository _participantRepository;

        public TourRequestParticipantManager(ITourRequestParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
        }

        public void SaveParticipants(int requestId, List<TourRequestParticipantDTO> participants)
        {
            foreach (var participantDTO in participants)
            {
                participantDTO.TourRequestId = requestId;
                var participant = participantDTO.ToTourRequestParticipant();
                _participantRepository.Save(participant);
            }
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

        public void DeleteAllParticipants(int requestId)
        {
            _participantRepository.DeleteByTourRequestId(requestId);
        }
    }
}