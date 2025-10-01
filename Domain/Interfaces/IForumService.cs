using BookingApp.Services.DTO;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IForumService
    {
        List<ForumDTO> GetAll();
        ForumDTO GetById(int forumId);
    }
}