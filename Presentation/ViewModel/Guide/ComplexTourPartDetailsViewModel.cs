using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Utilities;
using MvvmHelpers;
using MvvmHelpers.Commands;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class ComplexTourPartDetailsViewModel : BaseViewModel
    {
        public string Name { get; }
        public string Location { get; }
        public DateTime DateFrom { get; }
        public DateTime DateTo { get; }
        public string Language { get; }
        public int NumberOfParticipants { get; }
        public string Description { get; }
        public string RequestBy { get; }
        public ICommand AcceptCommand { get; }
        public event Action<ComplexTourRequest>? TourAccepted;
        public ObservableCollection<TourRequestParticipant> Participants { get; }
        private ComplexTourRequest req;

        public ComplexTourPartDetailsViewModel(ComplexTourRequestPart part)
        {
            Name = part.City;
            Location = $"{part.City}, {part.Country}";
            DateFrom = part.DateFrom;
            DateTo = part.DateTo;
            Language = part.Language;
            NumberOfParticipants = part.Participants.Count;
            Description = part.Description;
            AcceptCommand = new RelayCommand(OnAccept);
            RequestBy = GetTouristName(part.TouristId);
            //Participants = new ObservableCollection<TourRequestParticipant>(part.Participants);


        }
        private string GetTouristName(int id)
        {
            UserRepository repo = new UserRepository();
            var name = repo.GetById(id).FirstName;
            name += " ";
            name += repo.GetById(id).LastName;
            return name;
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
