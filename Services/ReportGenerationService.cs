using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using System;
using System.Collections.Generic;

namespace BookingApp.Services
{
    public class ReportGenerationService : IReportGenerationService
    {
        public ReportGenerationService() { }

        public void GenerateReservationReport(List<ReservationDetailsDTO> reservations, DateTime startDate, DateTime endDate)
        {
            try
            {
                var pdfService = new PdfReportService();
                pdfService.GenerateReservationReport(reservations, startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while generating the PDF report: {ex.Message}");
            }
        }
    }
}