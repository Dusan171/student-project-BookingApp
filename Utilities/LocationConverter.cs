using BookingApp.Services.DTO;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class LocationConverter : IValueConverter
    {
        // Pretvara LocationDTO u string "City, Country" za prikaz u UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LocationDTO locationDto)
            {
                return $"{locationDto.City}, {locationDto.Country}";
            }
            return string.Empty;
        }

        // Pretvara string "City, Country" iz UI u LocationDTO
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string locationString)
            {
                var parts = locationString.Split(new[] { ',' }, 2, StringSplitOptions.TrimEntries);

                if (parts.Length == 2)
                {
                    return new LocationDTO
                    {
                        City = parts[0],
                        Country = parts[1]
                    };
                }
            }
            return null;
        }
    }
}
