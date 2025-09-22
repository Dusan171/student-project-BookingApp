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
            if (value is bool boolValue)
            {
                // Ako postoji parameter, koristi ga umesto default vrednosti
                if (parameter is string paramString && !string.IsNullOrEmpty(paramString))
                {
                    var parts = paramString.Split('|');
                    if (parts.Length == 2)
                    {
                        return boolValue ? parts[0] : parts[1];
                    }
                }

                // Fallback na default vrednosti
                return boolValue ? TrueText : FalseText;
            }

            return FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (parameter is string paramString && !string.IsNullOrEmpty(paramString))
                {
                    var parts = paramString.Split('|');
                    if (parts.Length == 2)
                    {
                        if (s.Equals(parts[0], StringComparison.OrdinalIgnoreCase))
                            return true;
                        if (s.Equals(parts[1], StringComparison.OrdinalIgnoreCase))
                            return false;
                    }
                }

                // Fallback
                if (s.Equals(TrueText, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (s.Equals(FalseText, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return false;
        }
    }
}