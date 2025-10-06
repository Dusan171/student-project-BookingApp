using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;
using System.Diagnostics;
using System.Windows;
using System.Text.RegularExpressions;
using System;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourSearchView : UserControl
    {
        public TourSearchViewModel ViewModel => (TourSearchViewModel)DataContext;

        public TourSearchView()
        {
            InitializeComponent();
            DataContext = new TourSearchViewModel();

            // Debug - proverite da li je ViewModel kreiran
            Debug.WriteLine($"TourSearchView: ViewModel created - {ViewModel != null}");
            if (ViewModel != null)
            {
                Debug.WriteLine($"SearchToursCommand exists: {ViewModel.SearchToursCommand != null}");
            }
        }

        public void RefreshData()
        {
            ViewModel?.RefreshTours();
        }

        public void SetSearchCriteria(string city = "", string country = "", string language = "")
        {
            if (ViewModel != null)
            {
                if (!string.IsNullOrEmpty(city))
                    ViewModel.SearchCity = city;
                if (!string.IsNullOrEmpty(country))
                    ViewModel.SearchCountry = country;
                if (!string.IsNullOrEmpty(language))
                    ViewModel.SearchLanguage = language;
            }
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Debug informacije kada se view učita
            Debug.WriteLine("TourSearchView loaded");
            Debug.WriteLine($"DataContext is: {DataContext?.GetType().Name}");
            if (ViewModel != null)
            {
                Debug.WriteLine($"SearchToursCommand can execute: {ViewModel.SearchToursCommand?.CanExecute(null)}");
                Debug.WriteLine($"IsLoading: {ViewModel.IsLoading}");
            }
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Cleanup when view is unloaded
            ViewModel?.Dispose();
        }

        // Uklonjen OnLanguageChanged handler - sada koristi običan ComboBox binding

        // Web-like validacija prilikom klika na pretraži
        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            ClearAllErrors();

            bool hasErrors = false;
            string errorMessages = "";

            // Validacija broja ljudi
            if (!string.IsNullOrWhiteSpace(ViewModel.SearchPeopleCount))
            {
                if (!int.TryParse(ViewModel.SearchPeopleCount, out int peopleCount) || peopleCount <= 0)
                {
                    ShowFieldError(PeopleCountError, "Broj ljudi mora biti pozitivan broj");
                    errorMessages += "• Broj ljudi mora biti pozitivan broj\n";
                    hasErrors = true;
                }
                else if (peopleCount > 50)
                {
                    ShowFieldError(PeopleCountError, "Maksimalal broj ljudi je 50");
                    errorMessages += "• Maksimalan broj ljudi je 50\n";
                    hasErrors = true;
                }
            }

            // Validacija trajanja
            if (!string.IsNullOrWhiteSpace(ViewModel.SearchDuration))
            {
                if (!double.TryParse(ViewModel.SearchDuration, out double duration) || duration <= 0)
                {
                    ShowFieldError(DurationError, "Trajanje mora biti pozitivan broj");
                    errorMessages += "• Trajanje mora biti pozitivan broj (sati)\n";
                    hasErrors = true;
                }
                else if (duration > 48)
                {
                    ShowFieldError(DurationError, "Maksimalno trajanje je 48 sati");
                    errorMessages += "• Maksimalno trajanje je 48 sati\n";
                    hasErrors = true;
                }
            }

            // Validacija grada - samo slova i razmaci
            if (!string.IsNullOrWhiteSpace(ViewModel.SearchCity))
            {
                if (!Regex.IsMatch(ViewModel.SearchCity, @"^[a-žA-Ž\s\-']+$"))
                {
                    ShowFieldError(CityError, "Grad može sadržavati samo slova, razmake i crticice");
                    errorMessages += "• Grad može sadržavati samo slova, razmake i crticice\n";
                    hasErrors = true;
                }
                else if (ViewModel.SearchCity.Length > 50)
                {
                    ShowFieldError(CityError, "Naziv grada je predugačak (maksimalno 50 karaktera)");
                    errorMessages += "• Naziv grada je predugačak (maksimalno 50 karaktera)\n";
                    hasErrors = true;
                }
            }

            // Validacija države - samo slova i razmaci
            if (!string.IsNullOrWhiteSpace(ViewModel.SearchCountry))
            {
                if (!Regex.IsMatch(ViewModel.SearchCountry, @"^[a-žA-Ž\s\-']+$"))
                {
                    ShowFieldError(CountryError, "Država može sadržavati samo slova, razmake i crticice");
                    errorMessages += "• Država može sadržavati samo slova, razmake i crticice\n";
                    hasErrors = true;
                }
                else if (ViewModel.SearchCountry.Length > 50)
                {
                    ShowFieldError(CountryError, "Naziv države je predugačak (maksimalno 50 karaktera)");
                    errorMessages += "• Naziv države je predugačak (maksimalno 50 karaktera)\n";
                    hasErrors = true;
                }
            }

            // Ako ima grešaka, prikaži overlay umesto MessageBox
            if (hasErrors)
            {
                ShowErrorOverlay("Molimo ispravite sledeće greške:\n\n" + errorMessages.TrimEnd('\n'));
                return;
            }

            // Ako nema grešaka, nastavi sa pretragom
            if (ViewModel.SearchToursCommand?.CanExecute(null) == true)
            {
                ViewModel.SearchToursCommand.Execute(null);
            }
        }

        // Validacija prilikom izlaska iz polja (real-time)
        private void ValidateField(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string fieldName = textBox.Name;
            string value = textBox.Text?.Trim();

            // Očisti prethodnu grešku za ovo polje
            switch (fieldName)
            {
                case nameof(CityTextBox):
                    HideFieldError(CityError);
                    if (!string.IsNullOrEmpty(value) && (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$") || value.Length > 50))
                    {
                        ShowFieldError(CityError, value.Length > 50 ? "Predugačak naziv (maks. 50 karaktera)" : "Samo slova, razmaci i crticice");
                    }
                    break;

                case nameof(CountryTextBox):
                    HideFieldError(CountryError);
                    if (!string.IsNullOrEmpty(value) && (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$") || value.Length > 50))
                    {
                        ShowFieldError(CountryError, value.Length > 50 ? "Predugačak naziv (maks. 50 karaktera)" : "Samo slova, razmaci i crticice");
                    }
                    break;

                case nameof(DurationTextBox):
                    HideFieldError(DurationError);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!double.TryParse(value, out double duration) || duration <= 0)
                        {
                            ShowFieldError(DurationError, "Mora biti pozitivan broj");
                        }
                        else if (duration > 48)
                        {
                            ShowFieldError(DurationError, "Maksimalno 48 sati");
                        }
                    }
                    break;

                case nameof(PeopleCountTextBox):
                    HideFieldError(PeopleCountError);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!int.TryParse(value, out int count) || count <= 0)
                        {
                            ShowFieldError(PeopleCountError, "Mora biti pozitivan broj");
                        }
                        else if (count > 50)
                        {
                            ShowFieldError(PeopleCountError, "Maksimalno 50 ljudi");
                        }
                    }
                    break;
            }
        }

        // Helper metode za web-like error handling
        private void ShowFieldError(TextBlock errorTextBlock, string message)
        {
            if (errorTextBlock != null)
            {
                errorTextBlock.Text = message;
                errorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void HideFieldError(TextBlock errorTextBlock)
        {
            if (errorTextBlock != null)
            {
                errorTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearAllErrors()
        {
            HideFieldError(CityError);
            HideFieldError(CountryError);
            HideFieldError(DurationError);
            HideFieldError(PeopleCountError);
        }

        // Web-like modal overlay umesto MessageBox
        private void ShowErrorOverlay(string message)
        {
            ErrorMessage.Text = message;
            ErrorOverlay.Visibility = Visibility.Visible;
        }

        private void CloseErrorOverlay(object sender, RoutedEventArgs e)
        {
            ErrorOverlay.Visibility = Visibility.Collapsed;
        }
    }
}