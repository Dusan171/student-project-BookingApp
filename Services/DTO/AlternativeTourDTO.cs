using System.Windows.Media;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class AlternativeTourDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LocationText { get; set; } = string.Empty;
        public string DurationText { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int AvailableSpots { get; set; }
        public string AvailableSpotsText { get; set; } = string.Empty;
        public Brush AvailableSpotsColor { get; set; } = Brushes.Black;

        public AlternativeTourDTO() { }

       
        public AlternativeTourDTO(Tour tour, int availableSpots)
        {
            Id = tour.Id;
            Name = tour.Name;
            LocationText = $"{tour.Location?.City ?? "Nepoznat grad"}, {tour.Location?.Country ?? "Nepoznata zemlja"}";
            DurationText = $"{tour.DurationHours:F1}h";
            Language = tour.Language;
            AvailableSpots = availableSpots; 
            AvailableSpotsText = $"Slobodnih mesta: {AvailableSpots}";
            AvailableSpotsColor = AvailableSpots > 0 ? Brushes.Green : Brushes.Red;

            System.Diagnostics.Debug.WriteLine($"Kreiran AlternativeTourDTO: {Name}, ID: {Id}, Dostupno: {AvailableSpots}");
        }

        public AlternativeTourDTO(Tour tour)
        {
            Id = tour.Id;
            Name = tour.Name;
            LocationText = $"{tour.Location?.City ?? "Nepoznat grad"}, {tour.Location?.Country ?? "Nepoznata zemlja"}";
            DurationText = $"{tour.DurationHours:F1}h";
            Language = tour.Language;
            AvailableSpots = tour.AvailableSpots;
            AvailableSpotsText = $"Slobodnih mesta: {AvailableSpots}";
            AvailableSpotsColor = AvailableSpots > 0 ? Brushes.Green : Brushes.Red;

            System.Diagnostics.Debug.WriteLine($"Kreiran AlternativeTourDTO (legacy): {Name}, ID: {Id}, Dostupno: {AvailableSpots}");
        }
    }
}