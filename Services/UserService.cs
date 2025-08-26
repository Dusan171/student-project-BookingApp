using BookingApp.Domain;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public UserDTO GetUserByUsername(string username)
        {
            return _userRepository.GetByUsername(username) is User user ? new UserDTO(user) : null;
        }
    }
}
