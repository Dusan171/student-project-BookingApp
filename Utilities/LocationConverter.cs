using BookingApp.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class LocationConverter : IValueConverter
    {
        // Converts Location object to string: "City, Country"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var location = value as Location;
            if (location != null)
            {
                if (string.IsNullOrWhiteSpace(location.City) && string.IsNullOrWhiteSpace(location.Country))
                    return string.Empty;

                return $"{location.City}, {location.Country}";
            }

            return string.Empty;
        }

        // Converts string "City, Country" back to Location object
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) return null;

            var locationString = value.ToString();
            var parts = locationString.Split(',');

            if (parts.Length < 2) return null; // Osiguraj da ima oba dela

            return new Location
            {
                City = parts[0].Trim(),
                Country = parts[1].Trim()
            };
        }
    }
}

