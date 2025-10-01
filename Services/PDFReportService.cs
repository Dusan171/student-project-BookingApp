using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using BookingApp.Services.DTO; 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BookingApp.Services
{
    public class PdfReportService
    {
        public void GenerateReservationReport(List<ReservationDetailsDTO> reservations, DateTime startDate, DateTime endDate)
        {
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filePath = Path.Combine(downloadsPath, "Downloads", $"ReservationReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            using (var writer = new PdfWriter(filePath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);

                    var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    document.Add(new Paragraph("Reservation Report")
                        .SetFont(boldFont) 
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20));

                    document.Add(new Paragraph($"Period: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}")
                        .SetTextAlignment(TextAlignment.CENTER).SetFontSize(14));
                    document.Add(new Paragraph("\n"));

                    var table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 2, 2, 1, 1 }));
                    table.UseAllAvailableWidth();

                    table.AddHeaderCell(new Cell().Add(new Paragraph("Accommodation").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Start Date").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("End Date").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Duration (days)").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Guests").SetFont(boldFont)));

                    foreach (var res in reservations)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(res.AccommodationName)));
                        table.AddCell(new Cell().Add(new Paragraph(res.StartDate.ToString("dd.MM.yyyy"))));
                        table.AddCell(new Cell().Add(new Paragraph(res.EndDate.ToString("dd.MM.yyyy"))));
                        table.AddCell(new Cell().Add(new Paragraph($"{(res.EndDate - res.StartDate).Days}")));
                        table.AddCell(new Cell().Add(new Paragraph(res.GuestsNumber.ToString())));
                    }
                    document.Add(table);

                    document.Add(new Paragraph($"\nReport generated on: {DateTime.Now:dd.MM.yyyy HH:mm}")
                        .SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10));
                }
            }

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true };
            p.Start();
        }
    }
}