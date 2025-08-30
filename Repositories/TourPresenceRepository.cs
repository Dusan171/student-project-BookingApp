using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Domain.Interfaces;
using BookingApp.Domain.Model;
using BookingApp.Serializer;

namespace BookingApp.Repositories
{
    public class TourPresenceRepository : ITourPresenceRepository
    {
        private const string FilePath = "../../../Resources/Data/tour_presence.csv";
        private readonly Serializer<TourPresence> _serializer;
        private List<TourPresence> _tourPresences;

        public TourPresenceRepository()
        {
            _serializer = new Serializer<TourPresence>();
            _tourPresences = _serializer.FromCSV(FilePath);
        }

        public TourPresence GetById(int id)
        {
            return _tourPresences.FirstOrDefault(tp => tp.Id == id);
        }

        public List<TourPresence> GetAll()
        {
            return _tourPresences.ToList();
        }

        public TourPresence Save(TourPresence tourPresence)
        {
            tourPresence.Id = NextId();
            _tourPresences.Add(tourPresence);
            _serializer.ToCSV(FilePath, _tourPresences);
            return tourPresence;
        }

        public TourPresence Update(TourPresence tourPresence)
        {
            var existingPresence = GetById(tourPresence.Id);
            if (existingPresence != null)
            {
                existingPresence.TourId = tourPresence.TourId;
                existingPresence.UserId = tourPresence.UserId;
                existingPresence.JoinedAt = tourPresence.JoinedAt;
                existingPresence.IsPresent = tourPresence.IsPresent;
                existingPresence.CurrentKeyPointIndex = tourPresence.CurrentKeyPointIndex;
                existingPresence.LastUpdated = DateTime.Now;

                _serializer.ToCSV(FilePath, _tourPresences);
                return existingPresence;
            }
            return null;
        }

        public void Delete(TourPresence tourPresence)
        {
            _tourPresences.RemoveAll(tp => tp.Id == tourPresence.Id);
            _serializer.ToCSV(FilePath, _tourPresences);
        }

        public List<TourPresence> GetByTourId(int tourId)
        {
            return _tourPresences.Where(tp => tp.TourId == tourId).ToList();
        }

        public List<TourPresence> GetByUserId(int userId)
        {
            return _tourPresences.Where(tp => tp.UserId == userId).ToList();
        }

        public TourPresence GetByTourAndUser(int tourId, int userId)
        {
            return _tourPresences.FirstOrDefault(tp => tp.TourId == tourId && tp.UserId == userId);
        }

        public List<TourPresence> GetActivePresentUsers(int tourId)
        {
            return _tourPresences.Where(tp => tp.TourId == tourId && tp.IsPresent).ToList();
        }

        private int NextId()
        {
            return _tourPresences.Count > 0 ? _tourPresences.Max(tp => tp.Id) + 1 : 1;
        }
    }
}
