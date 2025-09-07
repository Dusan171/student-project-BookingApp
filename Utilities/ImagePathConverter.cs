using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace BookingApp.Utilities
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string relativePath && !string.IsNullOrEmpty(relativePath))
            {
                try
                {
                    // Konvertuj relativnu putanju u apsolutnu
                    string absolutePath = Path.GetFullPath(relativePath);

                    // Proveri da li fajl postoji
                    if (File.Exists(absolutePath))
                    {
                        return absolutePath;
                    }
                }
                catch
                {
                    
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}