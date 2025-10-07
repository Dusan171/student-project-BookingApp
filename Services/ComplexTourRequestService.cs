using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Services
{
    public class ComplexTourRequestService : IComplexTourRequestService
    {
        private readonly IComplexTourRequestRepository _requestRepository;
        private readonly IComplexTourRequestPartRepository _partRepository;
        private readonly IComplexTourRequestParticipantRepository _participantRepository;

        public ComplexTourRequestService(IComplexTourRequestRepository requestRepository,
                                       IComplexTourRequestPartRepository partRepository,
                                       IComplexTourRequestParticipantRepository participantRepository)
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _partRepository = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
            _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
        }

        public List<ComplexTourRequestDTO> GetAllRequests()
        {
            try
            {
                var requests = _requestRepository.GetAll();
                return EnrichRequestsAndConvertToDTO(requests);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju zahteva: {ex.Message}", ex);
            }
        }

        public ComplexTourRequestDTO GetRequestById(int id)
        {
            try
            {
                var request = _requestRepository.GetById(id);
                return request == null ? null : EnrichRequestAndConvertToDTO(request);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju zahteva sa ID {id}: {ex.Message}", ex);
            }
        }

        public ComplexTourRequestDTO CreateRequest(ComplexTourRequestDTO requestDTO)
        {
            if (requestDTO == null)
                throw new ArgumentNullException(nameof(requestDTO));

            if (!ValidateRequest(requestDTO))
                throw new ArgumentException("Neispravni podaci za složeni zahtev");

            try
            {
                var request = requestDTO.ToComplexTourRequest();

                if (requestDTO.Parts.Any())
                {
                    var earliestDate = requestDTO.Parts.Min(p => p.DateFrom);
                    request.InvalidationDeadline = earliestDate.AddHours(-48);
                }

                var savedRequest = _requestRepository.Save(request);

                for (int i = 0; i < requestDTO.Parts.Count; i++)
                {
                    var partDTO = requestDTO.Parts[i];
                    partDTO.ComplexTourRequestId = savedRequest.Id;
                    partDTO.PartIndex = i;

                    partDTO.TouristId = requestDTO.TouristId; 

                    var part = partDTO.ToComplexTourRequestPart();
                    var savedPart = _partRepository.Save(part);

                    // Sačuvaj učesnike za ovaj deo
                    foreach (var participantDTO in partDTO.Participants)
                    {

                        participantDTO.ComplexTourRequestPartId = savedPart.Id;

                        var participant = participantDTO.ToComplexTourRequestParticipant();
                        _participantRepository.Save(participant);
                    }
                }

                return EnrichRequestAndConvertToDTO(savedRequest);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri kreiranju zahteva: {ex.Message}", ex);
            }
        }

        public ComplexTourRequestDTO UpdateRequest(ComplexTourRequestDTO requestDTO)
        {
            if (requestDTO == null)
                throw new ArgumentNullException(nameof(requestDTO));

            try
            {
                var request = requestDTO.ToComplexTourRequest();
                var updatedRequest = _requestRepository.Update(request);
                return EnrichRequestAndConvertToDTO(updatedRequest);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri ažuriranju zahteva: {ex.Message}", ex);
            }
        }

        public void DeleteRequest(int id)
        {
            try
            {
                var request = _requestRepository.GetById(id);
                if (request != null)
                {
                    var parts = _partRepository.GetByComplexRequestId(id);
                    foreach (var part in parts)
                    {
                        _participantRepository.DeleteByPartId(part.Id);
                    }

                    _partRepository.DeleteByComplexRequestId(id);

                    _requestRepository.Delete(request);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri brisanju zahteva sa ID {id}: {ex.Message}", ex);
            }
        }

        public List<ComplexTourRequestDTO> GetRequestsByTourist(int touristId)
        {
            try
            {
                var requests = _requestRepository.GetByTouristId(touristId);
                return EnrichRequestsAndConvertToDTO(requests);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju zahteva za turista {touristId}: {ex.Message}", ex);
            }
        }

        public List<ComplexTourRequestDTO> GetPendingRequests()
        {
            try
            {
                var requests = _requestRepository.GetPendingRequests();
                return EnrichRequestsAndConvertToDTO(requests);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju zahteva na čekanju: {ex.Message}", ex);
            }
        }

        public void CheckAndMarkExpiredRequests()
        {
            try
            {
                var currentDateTime = DateTime.Now;

                // Dobij sve PENDING zahteve
                var pendingRequests = _requestRepository.GetAll()
                    .Where(r => r.Status == ComplexTourRequestStatus.PENDING)
                    .ToList();

                foreach (var request in pendingRequests)
                {
                    if (currentDateTime > request.InvalidationDeadline)
                    {
                        request.Status = ComplexTourRequestStatus.INVALID;
                        _requestRepository.Update(request);

                        var parts = _partRepository.GetByComplexRequestId(request.Id);
                        foreach (var part in parts)
                        {
                            if (part.Status == TourRequestStatus.PENDING)
                            {
                                part.Status = TourRequestStatus.INVALID;
                                _partRepository.Update(part);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri označavanju isteklih zahteva: {ex.Message}", ex);
            }
        }

        public ComplexTourRequestPartDTO AddPartToRequest(int requestId, ComplexTourRequestPartDTO partDTO)
        {
            if (partDTO == null)
                throw new ArgumentNullException(nameof(partDTO));

            try
            {
                var request = _requestRepository.GetById(requestId);
                if (request == null)
                    throw new ArgumentException("Složeni zahtev nije pronađen");

                if (request.Status != ComplexTourRequestStatus.PENDING)
                    throw new InvalidOperationException("Mogu se dodati delovi samo na zahteve na čekanju");

                var existingParts = _partRepository.GetByComplexRequestId(requestId);
                partDTO.ComplexTourRequestId = requestId;
                partDTO.PartIndex = existingParts.Count;

                var part = partDTO.ToComplexTourRequestPart();
                var savedPart = _partRepository.Save(part);

                foreach (var participantDTO in partDTO.Participants)
                {
                    participantDTO.ComplexTourRequestPartId = savedPart.Id;
                    var participant = participantDTO.ToComplexTourRequestParticipant();
                    _participantRepository.Save(participant);
                }

                UpdateInvalidationDeadline(requestId);

                return ComplexTourRequestPartDTO.FromDomain(savedPart);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri dodavanju dela zahteva: {ex.Message}", ex);
            }
        }

        public void RemovePartFromRequest(int partId)
        {
            try
            {
                var part = _partRepository.GetById(partId);
                if (part != null)
                {
                    _participantRepository.DeleteByPartId(partId);

                    _partRepository.Delete(part);

                    ReorganizePartIndices(part.ComplexTourRequestId);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri uklanjanju dela zahteva sa ID {partId}: {ex.Message}", ex);
            }
        }

        public List<ComplexTourRequestPartDTO> GetPartsByRequest(int requestId)
        {
            try
            {
                var parts = _partRepository.GetByComplexRequestId(requestId);
                var dtos = new List<ComplexTourRequestPartDTO>();

                foreach (var part in parts)
                {
                    part.Participants = _participantRepository.GetByPartId(part.Id);
                    dtos.Add(ComplexTourRequestPartDTO.FromDomain(part));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju delova za zahtev {requestId}: {ex.Message}", ex);
            }
        }

        public ComplexTourRequestParticipantDTO AddParticipant(int partId, ComplexTourRequestParticipantDTO participantDTO)
        {
            if (participantDTO == null)
                throw new ArgumentNullException(nameof(participantDTO));

            try
            {
                participantDTO.ComplexTourRequestPartId = partId;
                var participant = participantDTO.ToComplexTourRequestParticipant();
                var savedParticipant = _participantRepository.Save(participant);

                return ComplexTourRequestParticipantDTO.FromDomain(savedParticipant);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri dodavanju učesnika: {ex.Message}", ex);
            }
        }

        public void RemoveParticipant(int participantId)
        {
            try
            {
                var participant = _participantRepository.GetById(participantId);
                if (participant != null)
                {
                    _participantRepository.Delete(participant);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri uklanjanju učesnika sa ID {participantId}: {ex.Message}", ex);
            }
        }

        public List<ComplexTourRequestParticipantDTO> GetParticipantsByPart(int partId)
        {
            try
            {
                var participants = _participantRepository.GetByPartId(partId);
                return participants.Select(p => ComplexTourRequestParticipantDTO.FromDomain(p)).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Greška pri preuzimanju učesnika za deo {partId}: {ex.Message}", ex);
            }
        }

        private bool ValidateRequest(ComplexTourRequestDTO requestDTO)
        {
            if (requestDTO.Parts == null || !requestDTO.Parts.Any())
                return false;

            foreach (var part in requestDTO.Parts)
            {
                if (string.IsNullOrWhiteSpace(part.City) ||
                    string.IsNullOrWhiteSpace(part.Country) ||
                    string.IsNullOrWhiteSpace(part.Language) ||
                    part.NumberOfPeople <= 0 ||
                    part.DateFrom <= DateTime.Now.AddDays(2) ||
                    part.DateTo <= part.DateFrom ||
                    part.Participants.Count != part.NumberOfPeople)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateInvalidationDeadline(int requestId)
        {
            var request = _requestRepository.GetById(requestId);
            var parts = _partRepository.GetByComplexRequestId(requestId);

            if (request != null && parts.Any())
            {
                var earliestDate = parts.Min(p => p.DateFrom);
                request.InvalidationDeadline = earliestDate.AddHours(-48);
                _requestRepository.Update(request);
            }
        }

        private void ReorganizePartIndices(int complexRequestId)
        {
            var parts = _partRepository.GetByComplexRequestId(complexRequestId);
            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i].PartIndex != i)
                {
                    parts[i].PartIndex = i;
                    _partRepository.Update(parts[i]);
                }
            }
        }

        private List<ComplexTourRequestDTO> EnrichRequestsAndConvertToDTO(List<ComplexTourRequest> requests)
        {
            var dtos = new List<ComplexTourRequestDTO>();
            foreach (var request in requests)
            {
                dtos.Add(EnrichRequestAndConvertToDTO(request));
            }
            return dtos;
        }

        private ComplexTourRequestDTO EnrichRequestAndConvertToDTO(ComplexTourRequest request)
        {
            // Učitaj delove
            var parts = _partRepository.GetByComplexRequestId(request.Id);

            // Za svaki deo učitaj učesnike
            foreach (var part in parts)
            {
                part.Participants = _participantRepository.GetByPartId(part.Id);
            }

            request.Parts = parts.OrderBy(p => p.PartIndex).ToList();

            // Ažuriraj status na osnovu delova
            request.UpdateStatus();
            _requestRepository.Update(request);

            return ComplexTourRequestDTO.FromDomain(request);
        }

       
    }
}