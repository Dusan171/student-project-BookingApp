using System;
using System.Linq;
using System.Windows;

namespace BookingApp.Services
{
    public enum AppTheme { Light, Dark }

    public class ThemeService
    {
        public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;
        public void ChangeTheme(AppTheme theme)
        {
            CurrentTheme = theme;
            var themeUri = theme == AppTheme.Light
                ? new Uri("Themes/LightTheme.xaml", UriKind.Relative)
                : new Uri("Themes/DarkTheme.xaml", UriKind.Relative);

            var newThemeDictionary = new ResourceDictionary { Source = themeUri };

            var existingTheme = App.Current.Resources.MergedDictionaries
                                    .FirstOrDefault(rd => rd.Source != null && rd.Source.OriginalString.Contains("Theme.xaml"));

            if (existingTheme != null)
            {
                App.Current.Resources.MergedDictionaries.Remove(existingTheme);
            }

            App.Current.Resources.MergedDictionaries.Add(newThemeDictionary);
        }
    }
}