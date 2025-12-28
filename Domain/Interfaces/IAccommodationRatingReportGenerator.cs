using BookingApp.Services.DTO;
using System.Collections.Generic;
using System;

public interface IAccommodationRatingReportGenerator
{
    string Generate(List<AccommodationRatingDTO> ratings, DateTime fromDate, DateTime toDate, string ownerName);
}