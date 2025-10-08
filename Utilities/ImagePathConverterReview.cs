using System;
using System.Globalization;
using System.IO;

namespace BookingApp.Utilities
{
    public class ImagePathConverterReview
    {
        // NOVA METODA: Izdvojena logika pronalaženja putanje.
        // Nema uticaj na MELOC/CYCLO_SWITCH originalne Convert metode.
        private string FindFullPath(string relativePath)
        {
            if (!relativePath.StartsWith("Resources", StringComparison.OrdinalIgnoreCase))
            {
                relativePath = Path.Combine("Resources", "Images", relativePath);
            }

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDirectory, relativePath);
            fullPath = Path.GetFullPath(fullPath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            fullPath = Path.Combine(baseDirectory, "..", "..", "..", relativePath);
            fullPath = Path.GetFullPath(fullPath);

            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            return null; 
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return null;
            }

            try
            {
                string relativePath = value.ToString();

                return FindFullPath(relativePath);
            }
            catch
            {
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}