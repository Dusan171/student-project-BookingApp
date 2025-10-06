// ComplexTourRequestParticipantRepository.cs
using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Repositories
{
    public class ComplexTourRequestParticipantRepository : IComplexTourRequestParticipantRepository
    {
        private const string FilePath = "../../../Resources/Data/complex_tour_request_participants.csv";
        private readonly Serializer<ComplexTourRequestParticipant> _serializer;
        private List<ComplexTourRequestParticipant> _participants;

        public ComplexTourRequestParticipantRepository()
        {
            _serializer = new Serializer<ComplexTourRequestParticipant>();
            _participants = _serializer.FromCSV(FilePath);
        }

        public ComplexTourRequestParticipant GetById(int id)
        {
            return _participants.FirstOrDefault(p => p.Id == id);
        }

        public List<ComplexTourRequestParticipant> GetAll()
        {
            return _participants.ToList();
        }

        public ComplexTourRequestParticipant Save(ComplexTourRequestParticipant participant)
        {
            participant.Id = NextId();
            _participants.Add(participant);
            _serializer.ToCSV(FilePath, _participants);
            return participant;
        }

        public ComplexTourRequestParticipant Update(ComplexTourRequestParticipant participant)
        {
            var existingParticipant = GetById(participant.Id);
            if (existingParticipant != null)
            {
                existingParticipant.ComplexTourRequestPartId = participant.ComplexTourRequestPartId;
                existingParticipant.FirstName = participant.FirstName;
                existingParticipant.LastName = participant.LastName;
                existingParticipant.Age = participant.Age;
                _serializer.ToCSV(FilePath, _participants);
                return existingParticipant;
            }
            return null;
        }

        public void Delete(ComplexTourRequestParticipant participant)
        {
            _participants.RemoveAll(p => p.Id == participant.Id);
            _serializer.ToCSV(FilePath, _participants);
        }

        public List<ComplexTourRequestParticipant> GetByPartId(int partId)
        {
            return _participants.Where(p => p.ComplexTourRequestPartId == partId).ToList();
        }

        public void DeleteByPartId(int partId)
        {
            _participants.RemoveAll(p => p.ComplexTourRequestPartId == partId);
            _serializer.ToCSV(FilePath, _participants);
        }

        private int NextId()
        {
            return _participants.Count > 0 ? _participants.Max(p => p.Id) + 1 : 1;
        }
    }
}