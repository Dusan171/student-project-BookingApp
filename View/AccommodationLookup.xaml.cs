using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Utilities;
using BookingApp.Domain;
using BookingApp.Repositories;

namespace BookingApp.View
{
    /// <summary>
    /// Interaction logic for AccommodationLookup.xaml
    /// </summary>
    public partial class AccommodationLookup : Window
    {
        private readonly AccommodationRepository _accommodationRepository;
        private List<Accommodation> _allAccommodations;
        public AccommodationLookup()
        {
            InitializeComponent();
            _accommodationRepository = new AccommodationRepository();
            //_allAccommodations = new List<Accommodation>();
            _allAccommodations = _accommodationRepository.GetAll();
           // MessageBox.Show($"Ucitano smestaja: {_allAccommodations.Count}");
            AccommodationsDataGrid.ItemsSource = _allAccommodations;
        }
      /*  private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var TextBox = sender as TextBox;
            if (TextBox == null) return;

            //Brise samo ako je placeholder text (time je odradjeno da ne brise korisnicki unos slucajno)
            var placeholders = new List<string> { "Name","Country","City","Max Guests","Min Days"};
            if (placeholders.Contains(TextBox.Text))
            {
                TextBox.Text = "";
            }
        }*/

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string name = GetTrimedText(NameTextBox);
            string country = GetTrimedText(CountryTextBox);
            string city = GetTrimedText(CityTextBox);
            string selectedType = GetSelectedType();
            int maxGuests = TryParseInt(MaxGuestsTextBox.Text);
            int minDays = TryParseInt(MinDaysTextBox.Text);

            var  filtered = FilteredAccommodations(name, country, city, selectedType, maxGuests, minDays);
            AccommodationsDataGrid.ItemsSource = filtered;
        }
        private string GetTrimedText(TextBox textBox)
        {
            return textBox.Text.Trim();
        }
        private string GetSelectedType()
        {
            var selected = TypeComboBox.SelectedItem as ComboBoxItem;
            return selected?.Content.ToString();
        }
        private int TryParseInt(string input)
        {
            int.TryParse(input, out int value);
            return value;
        }
        private List<Accommodation> FilteredAccommodations(string name, string country, string city, string selectedType, int maxGuests, int minDays)
        {
            return _allAccommodations.Where(acc => MatchesFilter(acc, name, country, city, selectedType, maxGuests, minDays)).ToList();
        }
        private bool MatchesFilter(Accommodation acc, string name, string country, string city, string selectedType, int maxGuests, int minDays)
        {
            return NameMatches(acc, name) &&
                CountryMatches(acc, country) &&
                CityMatches(acc, city) &&
                TypeMatches(acc, selectedType) &&
                MaxGuestsMatches(acc, maxGuests) &&
                MinDaysMatches(acc, minDays);
        }
        private bool NameMatches(Accommodation acc, string name) =>
            string.IsNullOrEmpty(name) || acc.Name.Contains(name, StringComparison.OrdinalIgnoreCase);
        private bool CountryMatches(Accommodation acc, string country) =>
    string.IsNullOrEmpty(country) || acc.GeoLocation.Country.Contains(country, StringComparison.OrdinalIgnoreCase);

        private bool CityMatches(Accommodation acc, string city) =>
            string.IsNullOrEmpty(city) || acc.GeoLocation.City.Contains(city, StringComparison.OrdinalIgnoreCase);

        private bool TypeMatches(Accommodation acc, string type) =>
            type == "All" || acc.Type.ToString() == type;

        private bool MaxGuestsMatches(Accommodation acc, int maxGuests) =>
            maxGuests == 0 || acc.MaxGuests <= maxGuests;

        private bool MinDaysMatches(Accommodation acc, int minDays) =>
            minDays == 0 || acc.MinReservationDays >= minDays;

        public void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Session.CurrentUser = null;
                var signInWindow = new SignInForm();
                signInWindow.Show();
                //Zatvara trenutni prozor
                this.Close();
            }
        }
        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccommodation = (Accommodation)AccommodationsDataGrid.SelectedItem;

            if (selectedAccommodation == null)
            {
                MessageBox.Show("Please select an accommodation to reserve.");
                return;
            }
            try
            {
                var reservationWindow = new AccommodationReservationView(selectedAccommodation);
                reservationWindow.ShowDialog();
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            
        }
    }
}
