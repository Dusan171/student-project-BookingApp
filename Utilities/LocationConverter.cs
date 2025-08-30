using BookingApp.Services.DTO;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class LocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LocationDTO locationDto)
            {
                return FormatLocation(locationDto);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string locationString)
            {
                return ParseLocation(locationString);
            }
            return DependencyProperty.UnsetValue;
        }

        private string FormatLocation(LocationDTO location)
        {
            if (!IsLocationDtoValidForDisplay(location))
            {
                return string.Empty;
            }
            return $"{location.City}, {location.Country}";
        }

        private bool IsLocationDtoValidForDisplay(LocationDTO location)
        {
            if (string.IsNullOrWhiteSpace(location.City)) return false;
            if (string.IsNullOrWhiteSpace(location.Country)) return false;
            return true;
        }

        private object ParseLocation(string locationText)
        {
            if (string.IsNullOrWhiteSpace(locationText))
            {
                return DependencyProperty.UnsetValue;
            }

            var parts = locationText.Split(new[] { ',' }, 2);
            if (parts.Length != 2)
            {
                return DependencyProperty.UnsetValue;
            }

            string city = parts[0].Trim();
            string country = parts[1].Trim();

            if (!AreLocationPartsValid(city, country))
            {
                return DependencyProperty.UnsetValue;
            }

            return new LocationDTO { City = city, Country = country };
        }

        private bool AreLocationPartsValid(string city, string country)
        {
            if (string.IsNullOrEmpty(city)) return false;
            if (string.IsNullOrEmpty(country)) return false;
            return true;
        }
    }
}