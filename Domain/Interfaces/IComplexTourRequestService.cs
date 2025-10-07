using BookingApp.Domain.Model;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IComplexTourRequestService
    {
        List<ComplexTourRequestDTO> GetAllRequests();
        ComplexTourRequestDTO GetRequestById(int id);
        ComplexTourRequestDTO CreateRequest(ComplexTourRequestDTO requestDTO);
        ComplexTourRequestDTO UpdateRequest(ComplexTourRequestDTO requestDTO);
        void DeleteRequest(int id);
        List<ComplexTourRequestDTO> GetRequestsByTourist(int touristId);
        List<ComplexTourRequestDTO> GetPendingRequests();
        void CheckAndMarkExpiredRequests();

        // Part methods
        ComplexTourRequestPartDTO AddPartToRequest(int requestId, ComplexTourRequestPartDTO partDTO);
        void RemovePartFromRequest(int partId);
        List<ComplexTourRequestPartDTO> GetPartsByRequest(int requestId);

        // Participant methods  
        ComplexTourRequestParticipantDTO AddParticipant(int partId, ComplexTourRequestParticipantDTO participantDTO);
        void RemoveParticipant(int participantId);
        List<ComplexTourRequestParticipantDTO> GetParticipantsByPart(int partId);
    }
}