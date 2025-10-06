using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return "Unknown file";

            try
            {
                string path = value.ToString();
                return Path.GetFileName(path);
            }
            catch
            {
                return "Unknown file";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}