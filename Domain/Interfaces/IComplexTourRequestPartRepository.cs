using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IComplexTourRequestPartRepository
    {
        ComplexTourRequestPart GetById(int id);
        List<ComplexTourRequestPart> GetAll();
        ComplexTourRequestPart Save(ComplexTourRequestPart part);
        ComplexTourRequestPart Update(ComplexTourRequestPart part);
        void Delete(ComplexTourRequestPart part);
        List<ComplexTourRequestPart> GetByComplexRequestId(int complexRequestId);
        void DeleteByComplexRequestId(int complexRequestId);
    }
}