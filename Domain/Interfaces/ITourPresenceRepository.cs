using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourPresenceRepository
    {
        TourPresence GetById(int id);
        List<TourPresence> GetAll();
        TourPresence Save(TourPresence tourPresence);
        TourPresence Update(TourPresence tourPresence);
        void Delete(TourPresence tourPresence);
        List<TourPresence> GetByTourId(int tourId);
        List<TourPresence> GetByUserId(int userId);
        TourPresence GetByTourAndUser(int tourId, int userId);
        List<TourPresence> GetActivePresentUsers(int tourId);
    }
}
