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

        // Dodajte availableSpots kao parametar
        public AlternativeTourDTO(Tour tour, int availableSpots)
        {
            Id = tour.Id;
            Name = tour.Name;
            LocationText = tour.Location?.City ?? "Nepoznata lokacija";
            DurationText = $"{tour.DurationHours} h";
            Language = tour.Language;
            AvailableSpots = availableSpots; // Koristite prosleđenu vrednost
            AvailableSpotsText = $"{AvailableSpots} mesta";
            AvailableSpotsColor = AvailableSpots > 0 ? Brushes.Green : Brushes.Red;
        }

        // Za kompatibilnost - koristi Tour.AvailableSpots property
        public AlternativeTourDTO(Tour tour)
        {
            Id = tour.Id;
            Name = tour.Name;
            LocationText = tour.Location?.City ?? "Nepoznata lokacija";
            DurationText = $"{tour.DurationHours} h";
            Language = tour.Language;
            AvailableSpots = tour.AvailableSpots; // Koristi calculated property iz Tour modela
            AvailableSpotsText = $"{AvailableSpots} mesta";
            AvailableSpotsColor = AvailableSpots > 0 ? Brushes.Green : Brushes.Red;
        }
    }
}