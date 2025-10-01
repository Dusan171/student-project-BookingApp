using BookingApp.Domain.Model;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IForumRepository
    {
        Forum GetById(int id);
        List<Forum> GetAll();
        Forum Save(Forum forum);
        void Delete(Forum forum);
        Forum Update(Forum forum);
    }
}