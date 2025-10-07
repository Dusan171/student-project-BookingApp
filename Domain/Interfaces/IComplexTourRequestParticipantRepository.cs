using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IComplexTourRequestParticipantRepository
    {
        ComplexTourRequestParticipant GetById(int id);
        List<ComplexTourRequestParticipant> GetAll();
        ComplexTourRequestParticipant Save(ComplexTourRequestParticipant participant);
        ComplexTourRequestParticipant Update(ComplexTourRequestParticipant participant);
        void Delete(ComplexTourRequestParticipant participant);
        List<ComplexTourRequestParticipant> GetByPartId(int partId);
        void DeleteByPartId(int partId);
    }
}