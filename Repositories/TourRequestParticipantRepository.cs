using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;

namespace BookingApp.Repositories
{
    public class TourRequestParticipantRepository : ITourRequestParticipantRepository
    {
        private const string FilePath = "../../../Resources/Data/tour_request_participants.csv";
        private readonly Serializer<TourRequestParticipant> _serializer;
        private List<TourRequestParticipant> _participants;

        public TourRequestParticipantRepository()
        {
            _serializer = new Serializer<TourRequestParticipant>();
            _participants = _serializer.FromCSV(FilePath);
        }

        public TourRequestParticipant GetById(int id)
        {
            return _participants.FirstOrDefault(p => p.Id == id);
        }

        public List<TourRequestParticipant> GetAll()
        {
            return _participants.ToList();
        }

        public TourRequestParticipant Save(TourRequestParticipant participant)
        {
            participant.Id = NextId();
            _participants.Add(participant);
            _serializer.ToCSV(FilePath, _participants);
            return participant;
        }

        public TourRequestParticipant Update(TourRequestParticipant participant)
        {
            var existingParticipant = GetById(participant.Id);
            if (existingParticipant != null)
            {
                existingParticipant.TourRequestId = participant.TourRequestId;
                existingParticipant.FirstName = participant.FirstName;
                existingParticipant.LastName = participant.LastName;
                existingParticipant.Age = participant.Age;

                _serializer.ToCSV(FilePath, _participants);
                return existingParticipant;
            }
            return null;
        }

        public void Delete(TourRequestParticipant participant)
        {
            _participants.RemoveAll(p => p.Id == participant.Id);
            _serializer.ToCSV(FilePath, _participants);
        }

        public List<TourRequestParticipant> GetByTourRequestId(int tourRequestId)
        {
            return _participants.Where(p => p.TourRequestId == tourRequestId).ToList();
        }

        public void DeleteByTourRequestId(int tourRequestId)
        {
            _participants.RemoveAll(p => p.TourRequestId == tourRequestId);
            _serializer.ToCSV(FilePath, _participants);
        }

        private int NextId()
        {
            return _participants.Count > 0 ? _participants.Max(p => p.Id) + 1 : 1;
        }
    }
}
