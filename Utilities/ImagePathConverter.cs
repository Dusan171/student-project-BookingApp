using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace BookingApp.Utilities
{
    public class ImagePathConverter : IValueConverter
    {
        private string FindFullPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;

            string cleanedPath = relativePath.TrimStart('/', '\\');

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
            if (File.Exists(fullPath)) return fullPath;

            fullPath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", cleanedPath));
            if (File.Exists(fullPath)) return fullPath;

            Debug.WriteLine($"Slikа nije pronađena na: {fullPath}");
            return null;
        }
        private BitmapImage CreateBitmapFromPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return null;

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Bitmap creation error: {ex.Message}");
                return null;
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;

            try
            {
                string fullPath = FindFullPath(value.ToString());
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