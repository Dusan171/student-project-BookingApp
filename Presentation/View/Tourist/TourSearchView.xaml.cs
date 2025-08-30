using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;
using System.Diagnostics;
using System.Windows;

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

     
    }
}