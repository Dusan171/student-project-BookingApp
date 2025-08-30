using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourRequestService
    {
        List<TourRequestDTO> GetAllRequests();
        TourRequestDTO GetRequestById(int id);
        TourRequestDTO CreateRequest(TourRequestDTO requestDTO);
        TourRequestDTO UpdateRequest(TourRequestDTO requestDTO);
        void DeleteRequest(int id);
        List<TourRequestDTO> GetRequestsByTourist(int touristId);
        List<TourRequestDTO> GetPendingRequests();
        List<TourRequestDTO> GetRequestsByStatus(TourRequestStatus status);
        void AcceptRequest(int requestId, int guideId, DateTime scheduledDate);
        void MarkRequestAsInvalid(int requestId);
        void CheckAndMarkExpiredRequests();
        bool CanAcceptRequest(int requestId);

        // Participant methods
        TourRequestParticipantDTO AddParticipant(int requestId, TourRequestParticipantDTO participantDTO);
        void RemoveParticipant(int participantId);
        List<TourRequestParticipantDTO> GetParticipantsByRequest(int requestId);
    }
}
