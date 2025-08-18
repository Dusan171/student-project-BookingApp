using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Utilities;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.View;
using BookingApp.Services;
using BookingApp.Services.DTOs;

namespace BookingApp.Presentation.Guest
{
    /// <summary>
    /// Interaction logic for AccommodationLookup.xaml
    /// </summary>
    public partial class AccommodationLookup : Window
    {
        private readonly AccommodationRepository _accommodationRepository;
        private readonly AccommodationFilterService _filterService;
        public AccommodationLookup()
        {
            InitializeComponent();
            _accommodationRepository = new AccommodationRepository();
            _filterService = new AccommodationFilterService(_accommodationRepository);
            AccommodationsDataGrid.ItemsSource = _accommodationRepository.GetAll();
        }
     
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchParams = new AccommodationSearchParameters
            {
                Name = NameTextBox.Text.Trim(),
                Country = CountryTextBox.Text.Trim(), 
                City = CityTextBox.Text.Trim(),
                Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                MaxGuests = int.TryParse(MaxGuestsTextBox.Text,out int days) ? days : 0,
                MinDays = int.TryParse(MinDaysTextBox.Text, out int day) ? day : 0
            };

            var  filteredAccommodations = _filterService.Filter(searchParams);
            AccommodationsDataGrid.ItemsSource = filteredAccommodations;
        }
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
            if (AccommodationsDataGrid.SelectedItem is Accommodation selectedAccommodation)
            {
                var reservationWindow = new AccommodationReservationView(selectedAccommodation);
                reservationWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an accommodation to reserve.", "Selection Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}