using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public UserDTO() { }

        
        public UserDTO(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Role = user.Role.ToString();
        }

        
        public User ToUser()
        {
            return new User
            {
                Id = this.Id,
                Username = this.Username,
                Role = Enum.TryParse<UserRole>(this.Role, out var role) ? role : UserRole.GUEST
            };
        }
    }
}

