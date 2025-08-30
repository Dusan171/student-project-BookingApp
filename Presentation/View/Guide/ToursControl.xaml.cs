using BookingApp.Domain.Model;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using BookingApp.Utilities;
using BookingApp.Presentation.View.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ToursControl : UserControl
    {
        private List<Tour> allTours;
        private List<TourReservation> allReservations;
        MainPage mainPage;
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public ToursControl(MainPage main)
        {
            AllocConsole();
            InitializeComponent();

            LoadAllTours();
            LoadAllReservations();
            mainPage = main;

            TourFilterComboBox.SelectionChanged += TourFilterComboBox_SelectionChanged;

            DisplayTours(FilterToday());
        }

        private void LoadAllTours()
        {
            var tourRepository = new TourRepository();
            //ovde si menjala
            allTours = tourRepository.GetAll();
            FilterGuideTours();
            FillTourDetails(allTours);
        }

        private void FilterGuideTours()
        {
            allTours = allTours.Where(t => t.GuideId == Session.CurrentUser.Id).ToList();
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

                tour.KeyPoints = tour.KeyPoints
                    .Select(kp => keyPointRepo.GetById(kp.Id))
                    .Where(kp => kp != null)
                    .ToList();

                tour.StartTimes = tour.StartTimes
                    .Select(st => startTimeRepo.GetById(st.Id))
                    .Where(st => st != null)
                    .ToList();

                tour.Images = tour.Images
                    .Select(img => imagesRepo.GetById(img.Id))
                    .Where(img => img != null)
                    .ToList();
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
                bool isCancelled = IsCancelled(tour);

                //foreach (var startTime in tour.StartTimes)
                //{
                if (tour.StartTimes != null && tour.StartTimes.Any() && !isCancelled)
                {
                    CreateTourCard(tour, tour.StartTimes.First().Time, isToursToday, hasActiveReservation);
                }

                //}
            }
        }
        private bool IsCancelled(Tour tour)
        {
            return tour.Status == TourStatus.CANCELLED;
        }


        private bool HasOngoingTour()
        {
            return allTours.Any(t => t.Status == TourStatus.ACTIVE);
        }

        private bool IsNotFinished(Tour tour)
        {
            var foundTour = allTours.FirstOrDefault(t => t.Id == tour.Id);
            return foundTour != null && foundTour.Status == TourStatus.NONE;
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
                IsEnabled = hasActiveReservation && !HasOngoingTour() && IsNotFinished(tour)
            };

            startBtn.Click += (s, e) =>
            {
                if (!hasActiveReservation)
                {
                    MessageBox.Show("Tura ne može početi jer nema rezervisanih turista.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                tour.Status = TourStatus.ACTIVE;

                if (LiveTrackingFrame != null)
                {
                    var liveTrackingPage = new TourLiveTrackingControl(tour, time, mainPage);
                    LiveTrackingFrame.Navigate(liveTrackingPage);
                    LiveTrackingOverlay.Visibility = Visibility.Visible;
                    TourListPanel.Visibility = Visibility.Collapsed;
                }
            };
            Button cancelBtn = new Button
            {
                Content = "CANCEL",
                Foreground = Brushes.Red,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = !isToursToday ? Visibility.Visible : Visibility.Collapsed,
                IsEnabled = (time - DateTime.Now).TotalHours >= 48 && tour.Status != TourStatus.ACTIVE
            };

            cancelBtn.Click += (s, e) =>
            {
                var result = MessageBox.Show("Da li ste sigurni da želite otkazati ovu turu?", "Potvrda otkazivanja", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    tour.Status = TourStatus.CANCELLED;

                    var tourRepo = new TourRepository();
                    tourRepo.Update(tour);
                    MessageBox.Show("Tura je uspešno otkazana.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                    DisplayTours(TourFilterComboBox.SelectedIndex == 0 ? FilterToday() : allTours);
                }
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

            CreateTourControl form = new CreateTourControl(mainPage);
            form.TourCreated += (s, e) =>
            {
                LoadAllTours();
                DisplayTours(FilterToday());

            };
            form.Cancelled += OnCreateTourCancelled;
            mainPage.ContentFrame.Content = form;

        }

        private void OnCreateTourCancelled(object sender, EventArgs e)
        {
            LoadAllTours();
            DisplayTours(FilterToday());
            mainPage.ContentFrame.Content = this;
        }
    }
}
