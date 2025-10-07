using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface IUserService
    {
        UserDTO? GetUserByUsername(string username);
        UserDTO? GetUserById(int id);
        int GetCurrentUserId();
        string GetCurrentUsername();
    }
}