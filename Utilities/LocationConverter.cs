using System;
using System.Globalization;
using System.Windows.Data;
using BookingApp.Services.DTO;

namespace BookingApp.Utilities
{
    public class LocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LocationDTO location && location != null)
            {
                if (!string.IsNullOrWhiteSpace(location.City) && !string.IsNullOrWhiteSpace(location.Country))
                    return $"{location.City}, {location.Country}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text && !string.IsNullOrWhiteSpace(text))
            {
                var parts = text.Split(',');
                if (parts.Length == 2)
                    return new LocationDTO
                    {
                        City = parts[0].Trim(),
                        Country = parts[1].Trim()
                    };
            }
            return new LocationDTO();
        }
    }
}