using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BookingApp.Domain;
using System.Collections.ObjectModel;
using System.IO;
using BookingApp.Repositories;

namespace BookingApp.View.Guide
{
    public partial class CreateTourForm : Window
    {
        private List<KeyPoint> keyPoints = new();
        //private List<Images> images = new();
        private List<StartTourTime> startTimes = new();

        private readonly TourRepository tourRepository = new();
        private readonly KeyPointRepository keyPointRepository = new();
        private readonly ImageRepository imageRepository = new();
        private readonly StartTourTimeRepository startTimeRepository = new();
        private readonly LocationRepository locationRepository = new();

        private int _localKeyPointIdCounter = 0;
        private int _localStartTimeIdCounter = 0;
        private int _localImageIdCounter = 0;

        private ObservableCollection<ImagePaths> images = new ObservableCollection<ImagePaths>();


        public CreateTourForm()
        {
            InitializeComponent();
            ImagesListBox.ItemsSource = images;

        }

        private void AddKeyPoint_Click(object sender, RoutedEventArgs e)
        {
            string name = KeyPointTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(name) && !keyPoints.Exists(kp => kp.Name == name))
            {
                var newKeyPoint = new KeyPoint {Id = keyPointRepository.NextId() + _localKeyPointIdCounter++, Name = name };
                keyPoints.Add(newKeyPoint);
                KeyPointsListBox.Items.Add(newKeyPoint);
                KeyPointTextBox.Clear();
            }
        }

        private void RemoveKeyPoint_Click(object sender, RoutedEventArgs e)
        {
            if (KeyPointsListBox.SelectedItem is KeyPoint selected)
            {
                keyPoints.Remove(selected);
                KeyPointsListBox.Items.Remove(selected);
                _localKeyPointIdCounter--;
            }
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            string relativePath = ImagePathTextBox.Text.Trim();

            if (string.IsNullOrEmpty(relativePath))
            {
                MessageBox.Show("Please enter a path.");
                return;
            }

            string absolutePath = Path.IsPathRooted(relativePath)
                ? relativePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            if (!File.Exists(absolutePath))
            {
                MessageBox.Show("Image file not found.");
                return;
            }

            try
            {
                // Proveri da li već postoji
                foreach (var img in images)
                {
                    if (img.Path == relativePath)
                    {
                        MessageBox.Show("Image already added.");
                        return;
                    }
                }

                images.Add(new ImagePaths
                {
                    Id = imageRepository.NextId() + _localImageIdCounter++,
                    Path = relativePath
                }); ;

                ImagePathTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load image: " + ex.Message);
            }
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImagesListBox.SelectedItem is ImagePaths selectedImage)
            {
                images.Remove(selectedImage);
                _localImageIdCounter--;
            }
            else
            {
                MessageBox.Show("Select an image to remove.");
            }
        }




        private void AddStartTime_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null) return;

            if (!int.TryParse(StartTimeHourTextBox.Text, out int h)) return;
            if (!int.TryParse(StartTimeMinuteTextBox.Text, out int m)) return;

            DateTime date = StartDatePicker.SelectedDate.Value.Date + new TimeSpan(h, m, 0);

            if (!startTimes.Exists(t => t.Time == date))
            {
                var newStartTime = new StartTourTime { Id = startTimeRepository.NextId() + _localStartTimeIdCounter++, Time = date };
                startTimes.Add(newStartTime);
                StartTimesListBox.Items.Add(date.ToString("yyyy-MM-dd HH:mm"));
                StartTimeHourTextBox.Clear();
                StartTimeMinuteTextBox.Clear();
            }
        }

    private void RemoveStartTourTime_Click(object sender, RoutedEventArgs e)
        {
            if (StartTimesListBox.SelectedItem == null) return;

            string selectedString = StartTimesListBox.SelectedItem.ToString();
            if (!DateTime.TryParse(selectedString, out DateTime selectedDate)) return;

            var toRemove = startTimes.Find(t => t.Time == selectedDate);
            if (toRemove != null)
            {
                startTimes.Remove(toRemove);
                StartTimesListBox.Items.Remove(selectedString);
                _localStartTimeIdCounter--;
            }
        }



        private void CreateTour_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(CityTextBox.Text) ||
                string.IsNullOrWhiteSpace(CountryTextBox.Text) ||
                startTimes.Count == 0)
            {
                MessageBox.Show("Please fill in all required fields: Name, City, Country, and at least one Start Time.",
                                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TouristsTextBox.Text, out int maxTourists) || maxTourists < 1)
            {
                MessageBox.Show("Max Tourists must be a positive number.", "Validation Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(DurationTextBox.Text, out int duration) || duration < 1)
            {
                MessageBox.Show("Duration must be a positive number.", "Validation Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            Tour newTour = new Tour
            {
                Name = NameTextBox.Text.Trim(),
                Location = new Location
                {
                    City = CityTextBox.Text.Trim(),
                    Country = CountryTextBox.Text.Trim()
                },
                Description = DescriptionTextBox.Text.Trim(),
                Language = LanguageTextBox.Text.Trim(),
                MaxTourists = maxTourists,
                DurationHours = duration,
                StartTimes = new List<StartTourTime>(startTimes),
                KeyPoints = new List<KeyPoint>(keyPoints),
                Images = new List<ImagePaths>(images)
            };
            try
            {
                tourRepository.Save(newTour);
                MessageBox.Show("Tour created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                locationRepository.Save(newTour.Location);

                foreach (var startTime in newTour.StartTimes)
                {
                    startTimeRepository.Save(startTime);
                }

                foreach (var keyPoint in newTour.KeyPoints)
                {
                    keyPointRepository.Save(keyPoint);
                }

                foreach (var image in newTour.Images)
                {
                    imageRepository.Save(image);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
