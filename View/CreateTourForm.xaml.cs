using BookingApp.Model;
using BookingApp.Repository;
using System;
using System.Collections.Generic;
using System.Windows;
using BookingApp.Utilities;

namespace BookingApp.View
{
    public partial class CreateTourForm : Window
    {
        private List<KeyPoint> keyPoints = new();
        private List<Image> images = new();
        private List<StartTourTime> startTimes = new();

        private readonly TourRepository tourRepository = new();
        private readonly KeyPointRepository keyPointRepository = new();
        private readonly ImageRepository imageRepository = new();
        private readonly StartTourTimeRepository startTourTimeRepository = new();

        public CreateTourForm()
        {
            InitializeComponent();
        }

        private void AddKeyPoint_Click(object sender, RoutedEventArgs e)
        {
            string kpName = KeyPointNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(kpName))
            {
                MessageBox.Show("Please enter a KeyPoint name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var keyPoint = new KeyPoint { Name = kpName };
            keyPoints.Add(keyPoint);
            KeyPointsListBox.Items.Add(kpName);
            KeyPointNameTextBox.Clear();
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            string path = ImagePathTextBox.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("Please enter an image path.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var image = new Image { Path = path };
            images.Add(image);
            ImagesListBox.Items.Add(path);
            ImagePathTextBox.Clear();
        }

        private void AddStartTime_Click(object sender, RoutedEventArgs e)
        {
            if (StartTimeDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool validHour = int.TryParse(StartTimeHourTextBox.Text.Trim(), out int hour);
            bool validMinute = int.TryParse(StartTimeMinuteTextBox.Text.Trim(), out int minute);

            if (!validHour || !validMinute || hour < 0 || hour > 23 || minute < 0 || minute > 59)
            {
                MessageBox.Show("Please enter valid hour (0-23) and minute (0-59).", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime date = StartTimeDatePicker.SelectedDate.Value.Date
                            .AddHours(hour)
                            .AddMinutes(minute);

            var startTourTime = new StartTourTime { Time = date };
            startTimes.Add(startTourTime);
            StartTimesListBox.Items.Add(date.ToString("g"));

            StartTimeHourTextBox.Clear();
            StartTimeMinuteTextBox.Clear();
            StartTimeDatePicker.SelectedDate = null;
        }

        private void SaveTour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = NameTextBox.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Name cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(LocationIdTextBox.Text.Trim(), out int locationId))
                {
                    MessageBox.Show("Location ID must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string description = DescriptionTextBox.Text.Trim();
                string language = LanguageTextBox.Text.Trim();

                if (!int.TryParse(MaxTouristsTextBox.Text.Trim(), out int maxTourists))
                {
                    MessageBox.Show("Max Tourists must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(DurationHoursTextBox.Text.Trim(), out double durationHours))
                {
                    MessageBox.Show("Duration must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Save all related entities first to get their IDs
                foreach (var kp in keyPoints)
                {
                    keyPointRepository.Save(kp);
                }

                foreach (var img in images)
                {
                    imageRepository.Save(img);
                }

                foreach (var st in startTimes)
                {
                    startTourTimeRepository.Save(st);
                }

                var tour = new Tour
                {
                    Name = name,
                    Location = new Location { Id = locationId },
                    Description = description,
                    Language = language,
                    MaxTourists = maxTourists,
                    DurationHours = durationHours,
                    KeyPoints = keyPoints,
                    Images = images,
                    StartTimes = startTimes
                };

                tourRepository.Save(tour);

                MessageBox.Show("Tour saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save tour: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var signInWindow = new SignInForm();
            signInWindow.Show();
            this.Close();
        }
    }
}
