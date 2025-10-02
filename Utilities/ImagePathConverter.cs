using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BookingApp.Utilities
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;

            try
            {
                string relativePath = value.ToString();

                // Proveri da li putanja već počinje sa Resources/Images
                if (!relativePath.StartsWith("Resources", StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = Path.Combine("Resources", "Images", relativePath);
                }

                // Kreiraj apsolutnu putanju
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(baseDirectory, relativePath);

                // Normalizuj putanju
                fullPath = Path.GetFullPath(fullPath);

                if (!File.Exists(fullPath))
                {
                    // Pokušaj alternativnu putanju (3 nivoa gore)
                    fullPath = Path.Combine(baseDirectory, "..", "..", "..", relativePath);
                    fullPath = Path.GetFullPath(fullPath);
                }

                if (File.Exists(fullPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Za thread safety
                    return bitmap;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}