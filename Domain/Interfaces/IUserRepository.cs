using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTO;


namespace BookingApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        User? GetById(int id);
        public List<User> GetAll();
    }
}