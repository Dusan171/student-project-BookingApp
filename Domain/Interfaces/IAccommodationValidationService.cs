using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces.ServiceInterfaces
{
    public interface IAccommodationValidationService
    {
        bool IsAccommodationValid(AccommodationDTO accommodation,out string errorMessage);
    }
}