using System;
using System.Collections.Generic;
using BookingApp.Services.DTO;

namespace BookingApp.Domain.Interfaces.ServiceInterfaces
{
    public interface IPDFReportService
    {
       
        string GenerateAccommodationRatingsReport(List<AccommodationRatingDTO> ratings, DateTime fromDate, DateTime toDate, string ownerName);
    }
}
