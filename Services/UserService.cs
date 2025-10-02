using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserDTO? GetUserByUsername(string username)
        {
            var user = _userRepository.GetByUsername(username);
            return user != null ? new UserDTO(user) : null;
        }
       
        public UserDTO? GetUserById(int id)
        {
            var user = _userRepository.GetById(id);
            return user != null ? new UserDTO(user) : null;
        }

        
        public int GetCurrentUserId()
        {
            return Session.CurrentUser?.Id ?? 1; 
        }

        public string GetCurrentUsername()
        {
            return Session.CurrentUser?.Username ?? "Owner";
        }
    }
}

