using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookingApp.Utilities
{
    public class StarHighlightConverter : IValueConverter
    {
        // value = trenutna ocena, parameter = broj zvezdice
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Brushes.Gray;

            if (int.TryParse(value.ToString(), out int currentRating) &&
                int.TryParse(parameter.ToString(), out int starNumber))
            {
                return starNumber <= currentRating ? Brushes.Gold : Brushes.Gray;
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
