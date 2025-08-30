using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourPresenceService
    {
        List<TourPresenceDTO> GetAllPresences();
        TourPresenceDTO GetPresenceById(int id);
        TourPresenceDTO CreatePresence(TourPresenceDTO presenceDTO);
        TourPresenceDTO UpdatePresence(TourPresenceDTO presenceDTO);
        void DeletePresence(int id);
        List<TourPresenceDTO> GetPresencesByTour(int tourId);
        List<TourPresenceDTO> GetPresencesByUser(int userId);
        TourPresenceDTO GetUserPresenceOnTour(int tourId, int userId);
        TourPresenceDTO JoinTour(int tourId, int userId);
        TourPresenceDTO UpdateKeyPointProgress(int tourId, int userId, int keyPointIndex);
        List<TourPresenceDTO> GetActiveTourPresences(int userId);
        void MarkUserAsPresent(int tourId, int userId, bool isPresent);
        void NotifyUsersAboutPresence(int tourId, List<int> presentUserIds);
    }
}
