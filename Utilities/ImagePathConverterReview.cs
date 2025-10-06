using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Utilities
{
    public class ImagePathConverterReview
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;

            try
            {
                string relativePath = value.ToString();

                // Logika za pronalaženje fajla (ista kao u originalnom konverteru)
                if (!relativePath.StartsWith("Resources", StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = Path.Combine("Resources", "Images", relativePath);
                }

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(baseDirectory, relativePath);
                fullPath = Path.GetFullPath(fullPath);

                if (!File.Exists(fullPath))
                {
                    fullPath = Path.Combine(baseDirectory, "..", "..", "..", relativePath);
                    fullPath = Path.GetFullPath(fullPath);
                }

                if (File.Exists(fullPath))
                {
                    // KLJUČNA IZMENA: Samo vraća string putanju, NE kreira BitmapImage.
                    return fullPath;
                }

                return null;
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
