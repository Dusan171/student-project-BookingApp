using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BookingApp.Repository;
using System.Runtime.InteropServices;
using BookingApp.Repository;
using BookingApp.Model;
using BookingApp.View.Guide;

namespace BookingApp.View.Guide
{
    public partial class MainWindow : Window
    {
        private List<Tour> allTours;
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        private List<TourReservation> allReservations;


        public MainWindow()
        {
            AllocConsole();
            InitializeComponent();

            LoadAllTours();
            LoadAllReservations();
            TourFilterComboBox.SelectionChanged += TourFilterComboBox_SelectionChanged;

            DisplayTours(FilterToday());
        }
        private void LoadAllReservations()
        {
            var reservationRepository = new TourReservationRepository();
            allReservations = reservationRepository.GetAll();
        }

        private bool HasActiveReservation(Tour tour)
        {
            return allReservations.Any(r => r.TourId == tour.Id && r.IsActive);
        }

        private void LoadAllTours()
        {
            var tourRepository = new TourRepository();
            allTours = tourRepository.GetAll();

            foreach (var tour in allTours)
            {
                Console.WriteLine($"Tura: {tour.Name}");

                foreach (var start in tour.StartTimes)
                {
                    Console.WriteLine($" - Datum: {start.Time} (valid: {start.Time != DateTime.MinValue})");
                }
            }
        }
        private void NewTour_Click(object sender, RoutedEventArgs e)
        {
            CreateTourOverlay.Visibility = Visibility.Visible;
            TourListPanel.Visibility = Visibility.Collapsed;

            CreateTourForm form = new CreateTourForm();
            form.Cancelled += OnCreateTourCancelled;

            CreateTourFrame.Navigate(form);
        }

        private void OnCreateTourCancelled(object sender, System.EventArgs e)
        {
            CreateTourOverlay.Visibility = Visibility.Collapsed;
            TourListPanel.Visibility = Visibility.Visible;

            LoadAllTours();
        }
        private List<Tour> FilterToday()
        {
            DateTime today = DateTime.Today;

            return allTours.Where(tour =>
                tour.StartTimes.Any(time =>
                    time.Time.Date == today.Date)).ToList();
        }

        private void TourFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TourFilterComboBox.SelectedIndex == 0)
            {
                DisplayTours(FilterToday());
            }
            else
            {
                DisplayTours(allTours);
            }
        }

        private void DisplayTours(List<Tour> tours)
        {
            TourListPanel.Items.Clear();

            bool isToursToday = (TourFilterComboBox.SelectedIndex == 0);
           

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

            var date = time.ToString("dd.MM.yyyy, HH:mm");

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
            };

            Button cancelBtn = new Button
            {
                Content = "CANCEL",
                Foreground = Brushes.Red,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = isToursToday ? Visibility.Collapsed : Visibility.Visible
            };

            //cancelBtn.Click ti fali - tacka 4.

            horizontal.Children.Add(image);
            horizontal.Children.Add(info);
            horizontal.Children.Add(startBtn);
            horizontal.Children.Add(cancelBtn);

            card.Child = horizontal;

            TourListPanel.Items.Add(card);
        }


    }

}
