using BookingApp.Services;
using BookingApp.Utilities;
using System.Windows.Input;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ThemeService _themeService;
        private bool _isLightThemeChecked;
        public bool IsLightThemeChecked
        {
            get => _isLightThemeChecked;
            set { _isLightThemeChecked = value; OnPropertyChanged(); }
        }
        private bool _isDarkThemeChecked;
        public bool IsDarkThemeChecked
        {
            get => _isDarkThemeChecked;
            set { _isDarkThemeChecked = value; OnPropertyChanged(); }
        }

        private bool _isEnglishChecked;
        public bool IsEnglishChecked
        {
            get => _isEnglishChecked;
            set { _isEnglishChecked = value; OnPropertyChanged(); }
        }

        private bool _isSerbianChecked;
        public bool IsSerbianChecked
        {
            get => _isSerbianChecked;
            set { _isSerbianChecked = value; OnPropertyChanged(); }
        }


        public ICommand ChangeThemeCommand { get; }
        public ICommand ChangeLanguageCommand { get; }

        public SettingsViewModel()
        {
            _themeService = new ThemeService();
            ChangeThemeCommand = new RelayCommand(ChangeTheme);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);

            LoadCurrentTheme();
            LoadCurrentLanguage(); 
        }

        private void LoadCurrentTheme()
        {
            if (ThemeService.CurrentTheme == AppTheme.Dark)
            {
                IsDarkThemeChecked = true;
            }
            else
            {
                IsLightThemeChecked = true;
            }
        }

        private void LoadCurrentLanguage()
        {
            string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (currentCulture.StartsWith("sr"))
            {
                IsSerbianChecked = true;
            }
            else
            {
                IsEnglishChecked = true;
            }
        }

        private void ChangeTheme(object parameter)
        {
            if (parameter is string themeName)
            {
                var theme = themeName == "Dark" ? AppTheme.Dark : AppTheme.Light;
                _themeService.ChangeTheme(theme);
            }
        }
        private void ChangeLanguage(object parameter)
        {
            if (parameter is string culture)
            {
                LocalizationService.Instance.SetLanguage(culture);
            }
        }
    }
}