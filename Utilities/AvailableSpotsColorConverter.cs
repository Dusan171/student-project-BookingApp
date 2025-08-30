using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookingApp.Utilities
{
    public class AvailableSpotsColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int spots)
            {
                if (spots > 5)
                    return new SolidColorBrush(Colors.Green);
                else if (spots > 0)
                    return new SolidColorBrush(Colors.Orange);
                else
                    return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}