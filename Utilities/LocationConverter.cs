using BookingApp.Services.DTO; 
using System;
using System.Globalization;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class LocationConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LocationDTO locationDto)
            {
               
                return $"{locationDto.City}, {locationDto.Country}";
            }
            return string.Empty;
        }

        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string locationString)
            {
             
                var parts = locationString.Split(new[] { ',' }, 2);

                if (parts.Length == 2)
                {
                   
                    return new LocationDTO
                    {
                        City = parts[0].Trim(),
                        Country = parts[1].Trim()
                    };
                }
            }
            return null;
        }
    }
}