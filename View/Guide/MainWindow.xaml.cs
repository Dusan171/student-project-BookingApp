using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BookingApp.Domain;
using BookingApp.Domain.Model;
using BookingApp.Repositories;

namespace BookingApp.View.Guide
{
    public partial class MainWindow : Page
    {
        private List<Tour> allTours;
        private List<TourReservation> allReservations;

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public MainWindow()
        {
            AllocConsole();
            InitializeComponent();

            LoadAllTours();
            LoadAllReservations();

            TourFilterComboBox.SelectionChanged += TourFilterComboBox_SelectionChanged;

            DisplayTours(FilterToday());
        }

        private void LoadAllTours()
        {
            var tourRepository = new TourRepository();
            allTours = tourRepository.GetAll();
            FillTourDetails(allTours);
        }
        private void FillTourDetails(List<Tour> tours)
        {
            var locationRepo = new LocationRepository();
            var keyPointRepo = new KeyPointRepository();
            var startTimeRepo = new StartTourTimeRepository();
            var imagesRepo = new ImageRepository();

            foreach (var tour in tours)
            {
                if (tour.Location != null)
                    tour.Location = locationRepo.GetById(tour.Location.Id);

                var detailedKeyPoints = new List<KeyPoint>();
                foreach (var kp in tour.KeyPoints)
                {
                    var detailedKp = keyPointRepo.GetById(kp.Id);
                    if (detailedKp != null)
                        detailedKeyPoints.Add(detailedKp);
                }
                tour.KeyPoints = detailedKeyPoints;

                var detailedStartTimes = new List<StartTourTime>();
                foreach (var st in tour.StartTimes)
                {
                    var detailedSt = startTimeRepo.GetById(st.Id);
                    if (detailedSt != null)
                        detailedStartTimes.Add(detailedSt);
                }
                tour.StartTimes = detailedStartTimes;

                var detailedImages = new List<Images>();
                foreach (var img in tour.Images)
                {
                    var detailedImg = imagesRepo.GetById(img.Id);
                    if (detailedImg != null)
                        detailedImages.Add(detailedImg);
                }
                tour.Images = detailedImages;
            }
        }


        private void LoadAllReservations()
        {
            var reservationRepository = new TourReservationRepository();
            allReservations = reservationRepository.GetAll();
        }

        private bool HasActiveReservation(Tour tour)
        {
            return allReservations.Any(r => r.TourId == tour.Id && r.Status == 0);
        }

        private void TourFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TourFilterComboBox.SelectedIndex == 0)
                DisplayTours(FilterToday());
            else
                DisplayTours(allTours);
        }

        private List<Tour> FilterToday()
        {
            DateTime today = DateTime.Today;
            return allTours.Where(t => t.StartTimes.Any(st => st.Time.Date == today)).ToList();
        }

        private void DisplayTours(List<Tour> tours)
        {
            TourListPanel.Items.Clear();
            bool isToursToday = TourFilterComboBox.SelectedIndex == 0;

            foreach (var tour in tours)
            {
                bool hasActiveReservation = HasActiveReservation(tour);
                foreach (var startTime in tour.StartTimes)
                {
                    CreateTourCard(tour, startTime.Time, isToursToday, hasActiveReservation);
                }
            }
        }

        private void CreateTourCard(Tour tour, DateTime time, bool isToursToday, bool hasActiveReservation)
        {

            Border card = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(10)
            };

            StackPanel horizontal = new StackPanel { Orientation = Orientation.Horizontal };

            string imagePath = tour.Images?.FirstOrDefault()?.Path;
            BitmapImage bitmap;

            try
            {
                bitmap = !string.IsNullOrEmpty(imagePath) && File.Exists(imagePath)
                    ? new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute))
                    : new BitmapImage();
            }
            catch
            {
                bitmap = new BitmapImage();
            }

            Image image = new Image
            {
                Width = 60,
                Height = 60,
                Margin = new Thickness(0, 0, 10, 0),
                Source = bitmap,
                Stretch = Stretch.UniformToFill
            };

            StackPanel info = new StackPanel();
            string date = time.ToString("dd.MM.yyyy, HH:mm");
            info.Children.Add(new TextBlock { Text = tour.Name, FontWeight = FontWeights.Bold });
            info.Children.Add(new TextBlock { Text = date });
            info.Children.Add(new TextBlock { Text = tour.Language });
            info.Children.Add(new TextBlock { Text = $"Duration: {tour.DurationHours}h" });

            Button startBtn = new Button
            {
                Content = "START",
                Foreground = Brushes.Blue,
                Margin = new Thickness(20, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = isToursToday ? Visibility.Visible : Visibility.Collapsed,
                IsEnabled = !hasActiveReservation
            };

            startBtn.Click += (s, e) =>
            {
                var liveTrackingPage = new TourLiveTracking(tour, time);
                LiveTrackingFrame.Navigate(liveTrackingPage);
                LiveTrackingOverlay.Visibility = Visibility.Visible;
                TourListPanel.Visibility = Visibility.Collapsed;
            };

            Button cancelBtn = new Button
            {
                Content = "CANCEL",
                Foreground = Brushes.Red,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = isToursToday ? Visibility.Collapsed : Visibility.Visible
            };

            cancelBtn.Click += (s, e) =>
            {
                MessageBox.Show($"Dodaj kod");
            };

            horizontal.Children.Add(image);
            horizontal.Children.Add(info);
            horizontal.Children.Add(startBtn);
            horizontal.Children.Add(cancelBtn);

            card.Child = horizontal;

            TourListPanel.Items.Add(card);
        }

        private void NewTour_Click(object sender, RoutedEventArgs e)
        {
            CreateTourOverlay.Visibility = Visibility.Visible;
            TourListPanel.Visibility = Visibility.Collapsed;

            CreateTourForm form = new CreateTourForm();
            form.Cancelled += OnCreateTourCancelled;
            CreateTourFrame.Navigate(form);
        }

        private void OnCreateTourCancelled(object sender, EventArgs e)
        {
            CreateTourOverlay.Visibility = Visibility.Collapsed;
            TourListPanel.Visibility = Visibility.Visible;
            LoadAllTours();
            DisplayTours(FilterToday());
        }
    }
}