using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BookingApp.Model;
using BookingApp.Repository;

namespace BookingApp.View
{
    public partial class TourSearch : Window
    {
        public TourRepository _tourRepository = new TourRepository();

        public TourSearch()
        {
            InitializeComponent();
            _tourRepository = new TourRepository();
            LoadAllTours();
        }

        public void LoadAllTours()
        {
            ToursDataGrid.ItemsSource = _tourRepository.GetAll();
        }

        public void SearchTours_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text.Trim().ToLower();
            string country = CountryTextBox.Text.Trim().ToLower();
            string language = LanguageTextBox.Text.Trim().ToLower();

            double.TryParse(DurationTextBox.Text, out double maxDuration);
            int.TryParse(PeopleCountTextBox.Text, out int peopleCount);

            var filteredTours = _tourRepository.GetAll().Where(t =>
                (string.IsNullOrEmpty(city) || t.Location.City.ToLower().Contains(city)) &&
                (string.IsNullOrEmpty(country) || t.Location.Country.ToLower().Contains(country)) &&
                (string.IsNullOrEmpty(language) || t.Language.ToLower().Contains(language)) &&
                (maxDuration == 0 || t.DurationHours <= maxDuration) &&
                (peopleCount == 0 || (t.MaxTourists - t.ReservedSpots) >= peopleCount)
            ).ToList();

            ToursDataGrid.ItemsSource = filteredTours;
        }
    }
}
