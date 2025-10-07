using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookingApp.Domain.Model;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace BookingApp.Services
{
    public class TourReportService
    {
        static TourReportService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public string GenerateTourReport(List<Tour> tours, DateTime startDate, DateTime endDate, User guide)
        {
            string fileName = $"ScheduledTourReport_{guide.Id}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    page.Header()
                        .Text("SCHEDULED TOURS REPORT")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Item().Text($"Guide: {guide.FirstName} {guide.LastName} (ID: {guide.Id})").FontSize(12);
                            x.Item().Text($"Date generated: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(12);
                            x.Item().Text($"Period: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}").FontSize(12);

                            x.Item().PaddingTop(20).LineHorizontal(1);

                            x.Item().PaddingTop(20);

                            if (!tours.Any())
                            {
                                x.Item().Text("Nema zakazanih tura u zadatom periodu.").FontSize(12);
                            }
                            else
                            {
                                x.Item().Text("LISTA TURA:").FontSize(14).SemiBold();
                                x.Item().PaddingTop(10);

                                foreach (var tour in tours)
                                {
                                    x.Item().PaddingBottom(15).Column(tourColumn =>
                                    {
                                        tourColumn.Item().Text($"Name: {tour.Name}").FontSize(11).SemiBold();
                                        tourColumn.Item().Text($"Location: {tour.Location?.City}, {tour.Location?.Country}").FontSize(10);
                                        tourColumn.Item().Text($"Language: {tour.Language}").FontSize(10);
                                        tourColumn.Item().Text($"Max number of tourists: {tour.MaxTourists}").FontSize(10);
                                        tourColumn.Item().Text($"Reserved spots: {tour.ReservedSpots}").FontSize(10);
                                        tourColumn.Item().Text($"Available spots: {tour.AvailableSpots}").FontSize(10);
                                        tourColumn.Item().Text($"Duration: {tour.DurationHours} sati").FontSize(10);
                                        tourColumn.Item().Text($"Status: {GetStatusText(tour.Status)}").FontSize(10);

                                        if (tour.StartTimes?.Any() == true)
                                        {
                                            var startTimesInPeriod = tour.StartTimes
                                                .Where(st => st.Time.Date >= startDate.Date && st.Time.Date <= endDate.Date)
                                                .OrderBy(st => st.Time);

                                            if (startTimesInPeriod.Any())
                                            {
                                                tourColumn.Item().Text("Dates:").FontSize(10);
                                                foreach (var startTime in startTimesInPeriod)
                                                {
                                                    tourColumn.Item().PaddingLeft(10)
                                                        .Text($"• {startTime.Time:dd.MM.yyyy HH:mm}").FontSize(9);
                                                }
                                            }
                                        }

                                        if (tour.KeyPoints?.Any() == true)
                                        {
                                            tourColumn.Item().Text($"Number of key points: {tour.KeyPoints.Count}").FontSize(10);

                                            foreach (var keyPoint in tour.KeyPoints)
                                            {
                                                tourColumn.Item().PaddingLeft(10)
                                                    .Text($"• {keyPoint.Name}").FontSize(9);
                                            }
                                        }


                                        tourColumn.Item().PaddingTop(5).LineHorizontal(0.5f);
                                    });
                                }

                                x.Item().PaddingTop(20).Text("STATISTICS:").FontSize(14).SemiBold();
                                x.Item().PaddingTop(10).Column(statsColumn =>
                                {
                                    statsColumn.Item().Text($"Total number of tours: {tours.Count}").FontSize(11);

                                    var activeTours = tours.Count(t => t.Status == TourStatus.ACTIVE);
                                    statsColumn.Item().Text($"Active tours: {activeTours}").FontSize(11);

                                    var finishedTours = tours.Count(t => t.Status == TourStatus.FINISHED);
                                    statsColumn.Item().Text($"Finished tours: {finishedTours}").FontSize(11);

                                    var cancelledTours = tours.Count(t => t.Status == TourStatus.CANCELLED);
                                    statsColumn.Item().Text($"cancelled tours: {cancelledTours}").FontSize(11);
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }

        private string GetStatusText(TourStatus status)
        {
            return status switch
            {
                TourStatus.ACTIVE => "Active",
                TourStatus.FINISHED => "Finished",
                TourStatus.CANCELLED => "Cancelled",
                TourStatus.NONE => "Not Started",
                _ => "Undeifned"
            };
        }
    }
}