using BookingApp.Domain.Model;
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
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public UserDTO() { }

        public UserDTO(User user)
        {
            Id = user.Id;
            Username = user.Username ?? string.Empty;
            Role = user.Role.ToString();
            Email = string.Empty; // Set default as User model doesn't have Email
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