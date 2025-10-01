using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;

namespace BookingApp.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private static readonly LocalizationService _instance = new LocalizationService();
        public static LocalizationService Instance => _instance;

        private readonly ResourceManager _resourceManager;

        public string this[string key] => _resourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture);

        public event PropertyChangedEventHandler PropertyChanged;

        private LocalizationService()
        {
            _resourceManager = new ResourceManager("BookingApp.Resources.Strings", Assembly.GetExecutingAssembly());
        }

        public void SetLanguage(string culture)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
            catch (CultureNotFoundException)
            {
                MessageBox.Show("Language not supported.");
            }
        }
    }
}