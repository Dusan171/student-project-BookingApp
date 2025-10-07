using System;
using System.Globalization;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class StarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            int rating = System.Convert.ToInt32(value);

            if (rating < 0) rating = 0;
            if (rating > 5) rating = 5;

            return new string('★', rating) + new string('☆', 5 - rating);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
