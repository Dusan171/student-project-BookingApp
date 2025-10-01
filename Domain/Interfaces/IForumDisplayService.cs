using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces
{ 
    public interface IForumDisplayService
    {
        ForumDTO AssembleForumDTO(Forum forum);
    }
}