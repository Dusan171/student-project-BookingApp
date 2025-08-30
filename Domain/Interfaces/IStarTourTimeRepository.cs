using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IStartTourTimeRepository
    {
        List<StartTourTime> GetAll();
        StartTourTime? GetById(int id);
        StartTourTime Save(StartTourTime startTourTime);
        StartTourTime Update(StartTourTime startTourTime);
        void Delete(StartTourTime startTourTime);
        int GetNextId();
        int NextId();
        List<StartTourTime> GetByTourId(int tourId);
        void SaveAll();
    }
}