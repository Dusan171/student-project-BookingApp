using BookingApp.Domain.Model;
using BookingApp.Utilities;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BookingApp.Presentation.View.Guide
{
    public class TourRequestDetailsViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public int NumberOfPeople { get; set; }
        public string Description { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string RequestedBy { get; set; }
        public List<TourRequestParticipant> Participants { get; set; }
        public event Action<TourRequest>? TourAccepted;

        public ICommand AcceptCommand { get; }
        private TourRequest req;

        public TourRequestDetailsViewModel(TourRequest request)
        {
            City = request.City;
            Country = request.Country;
            Language = request.Language;
            NumberOfPeople = request.NumberOfPeople;
            Description = request.Description;
            req = request;

            DateFrom = request.DateFrom;
            DateTo = request.DateTo;

            RequestedBy = request.TouristName;
            Participants = request.Participants;

            AcceptCommand = new RelayCommand(OnAccept);
        }

        private void OnAccept()
        {
            var acceptWindow = new Window
            {
                Title = "Accept Tour",
                Width = 320,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.White,
                Owner = Application.Current.MainWindow
            };

            var mainPanel = new StackPanel
            {
                Margin = new Thickness(16),
                VerticalAlignment = VerticalAlignment.Center
            };

            mainPanel.Children.Add(new TextBlock
            {
                Text = "Select tour date:",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.Black,
                Margin = new Thickness(0, 0, 0, 8)
            });

            var dateCombo = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 12),
                Padding = new Thickness(4)
            };
            for (var d = DateFrom.Date; d <= DateTo.Date; d = d.AddDays(1))
                dateCombo.Items.Add(d.ToString("dd.MM.yyyy"));
            dateCombo.SelectedIndex = 0;
            mainPanel.Children.Add(dateCombo);

            var timePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 12)
            };
            timePanel.Children.Add(new TextBlock
            {
                Text = "Time:",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            });

            var hourCombo = new ComboBox { Width = 60, Margin = new Thickness(0, 0, 4, 0), Padding = new Thickness(2) };
            for (int h = 0; h < 24; h++) hourCombo.Items.Add(h.ToString("D2"));
            hourCombo.SelectedIndex = 9;
            timePanel.Children.Add(hourCombo);

            timePanel.Children.Add(new TextBlock { Text = ":", VerticalAlignment = VerticalAlignment.Center });

            var minuteCombo = new ComboBox { Width = 60, Margin = new Thickness(4, 0, 0, 0), Padding = new Thickness(2) };
            for (int m = 0; m < 60; m += 5) minuteCombo.Items.Add(m.ToString("D2"));
            minuteCombo.SelectedIndex = 0;
            timePanel.Children.Add(minuteCombo);

            mainPanel.Children.Add(timePanel);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 12, 0, 0)
            };

            var confirmButton = new Button
            {
                Content = "Confirm",
                Width = 100,
                Margin = new Thickness(4),
                Background = System.Windows.Media.Brushes.MediumPurple,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(6, 4, 6, 4)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 100,
                Margin = new Thickness(4),
                Background = System.Windows.Media.Brushes.Gray,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(6, 4, 6, 4)
            };

            buttonPanel.Children.Add(confirmButton);
            buttonPanel.Children.Add(cancelButton);
            mainPanel.Children.Add(buttonPanel);

            acceptWindow.Content = mainPanel;

            confirmButton.Click += (_, __) =>
            {
                var selectedDate = dateCombo.SelectedItem.ToString();
                var selectedHour = hourCombo.SelectedItem.ToString();
                var selectedMinute = minuteCombo.SelectedItem.ToString();

                //logika da li je vodič slobodan i rezervacija i na kraju napraviti turu sa ovim info!!!
                MessageBox.Show($"Successfully accepted the tour on {selectedDate} at {selectedHour}:{selectedMinute}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                acceptWindow.Close();
                TourAccepted?.Invoke(req);
            };

            cancelButton.Click += (_, __) => acceptWindow.Close();

            acceptWindow.ShowDialog();


        }

    }
}
