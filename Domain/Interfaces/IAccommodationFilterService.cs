using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Services.DTOs;

namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationFilterService
    {
        public List<Accommodation> Filter(AccommodationSearchParameters parameters);
    }
}
