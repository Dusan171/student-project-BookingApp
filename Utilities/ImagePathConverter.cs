using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Diagnostics; // Dodajemo za Debug.WriteLine

namespace BookingApp.Utilities
{
    public class ImagePathConverter : IValueConverter
    {
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
        private BitmapImage CreateBitmapFromPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze(); // Za thread safety

            return bitmap;
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
                string fullPath = FindFullPath(relativePath);
                return CreateBitmapFromPath(fullPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}