using System;
using System.Collections.Generic;
using BookingApp.Services.DTO;


namespace BookingApp.Domain.Interfaces
{
    public interface IAccommodationFilterService
    {
        public List<AccommodationDetailsDTO> Filter(AccommodationSearchParameters parameters);
    }
}
