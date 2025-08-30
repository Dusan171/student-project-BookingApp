using System;
using System.Globalization;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class BooleanToTextConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Da";
        public string FalseText { get; set; } = "Ne";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? TrueText : FalseText;

            return FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (s.Equals(TrueText, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (s.Equals(FalseText, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return false;
        }
    }
}
