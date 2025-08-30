using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IKeyPointRepository
    {
        List<KeyPoint> GetAll();
        KeyPoint GetById(int id);
        KeyPoint Save(KeyPoint keyPoint);
        void Delete(KeyPoint keyPoint);
        KeyPoint Update(KeyPoint keyPoint);
        int NextId();
    }
}
