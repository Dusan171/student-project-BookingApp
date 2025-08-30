using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;

namespace BookingApp.Domain.Interfaces
{
    public interface ITourRequestParticipantRepository
    {
        TourRequestParticipant GetById(int id);
        List<TourRequestParticipant> GetAll();
        TourRequestParticipant Save(TourRequestParticipant participant);
        TourRequestParticipant Update(TourRequestParticipant participant);
        void Delete(TourRequestParticipant participant);
        List<TourRequestParticipant> GetByTourRequestId(int tourRequestId);
        void DeleteByTourRequestId(int tourRequestId);
    }
}
