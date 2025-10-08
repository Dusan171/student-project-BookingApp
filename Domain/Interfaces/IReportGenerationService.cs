using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;

namespace BookingApp.Domain.Interfaces
{
    public interface IReportGenerationService
    {
        void GenerateReservationReport(List<ReservationDetailsDTO> reservations, DateTime startDate, DateTime endDate);
    }
}