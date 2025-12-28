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
                string inputPath = value.ToString();

                string cleanedPath = inputPath.TrimStart('/', '\\');

                if (!cleanedPath.Contains("Resources/Images", StringComparison.OrdinalIgnoreCase))
                {
                    if (cleanedPath.StartsWith("Images", StringComparison.OrdinalIgnoreCase))
                    {
                        cleanedPath = Path.Combine("Resources", cleanedPath);
                    }
                    else
                    {
                        cleanedPath = Path.Combine("Resources", "Images", cleanedPath);
                    }
                }

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string fullPath = Path.GetFullPath(Path.Combine(baseDirectory, cleanedPath));

                if (!File.Exists(fullPath))
                {
                    fullPath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", cleanedPath));
                }

                if (File.Exists(fullPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze(); 
                    return bitmap;
                }

                System.Diagnostics.Debug.WriteLine($"Slikа nije pronađena na: {fullPath}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Greška u konverteru: {ex.Message}");
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}