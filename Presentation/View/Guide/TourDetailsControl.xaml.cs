using System.Linq;
using System.Windows.Controls;
using BookingApp.Domain.Model;
using BookingApp.Repositories;

namespace BookingApp.Presentation.View.Guide
{
    public partial class TourDetailsControl : UserControl
    {
        private ReservationGuestRepository _guestRepository;
        private TouristAttendanceRepository _attendanceRepository;
        private Tour _tour;
        MainPage mainPage;

        public TourDetailsControl(Tour tour, MainPage main)
        {
            InitializeComponent();
            _tour = tour;
            _guestRepository = new ReservationGuestRepository();
            _attendanceRepository = new TouristAttendanceRepository();
            mainPage = main;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            TourNameText.Text = _tour.Name;
            foreach(var time in _tour.StartTimes)
            {
                TourDateText.Text = time.ToString();
            }
            TourDurationText.Text = $"Tour Duration: {_tour.DurationHours.ToString()}h"; 
            var attendance = _attendanceRepository.GetAll().Where(a => a.TourId == _tour.Id).ToList();
            var tourists = _guestRepository.GetAll()
                            .Where(t => attendance.Any(a => a.GuestId == t.Id))
                            .ToList();

            int under18 = tourists.Count(t => t.Age < 18);
            int between18and50 = tourists.Count(t => t.Age >= 18 && t.Age <= 50);
            int above50 = tourists.Count(t => t.Age > 50);

            int total = under18 + between18and50 + above50;
            TotalTouristText.Text = $"Total number of Tourists: {total}";
            if (total == 0) total = 1;
            Under18Text.Text = $"Under 18: {under18} ({(under18 * 100.0 / total):0.##}%)";
            Between18And50Text.Text = $"18-50: {between18and50} ({(between18and50 * 100.0 / total):0.##}%)";
            Above50Text.Text = $"Above 50: {above50} ({(above50 * 100.0 / total):0.##}%)";
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mainPage.ContentFrame.Content = new TourStatisticsControl(mainPage);
        }
    }
}

/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BookingApp.Domain;
using BookingApp.Repositories;

namespace BookingApp.View.Guide
{
    public partial class TourStatisticsPage : Page
    {
        private List<Tour> finishedTours;

        public TourStatisticsPage()
        {
            InitializeComponent();
            LoadFinishedTours();
            PopulateYearComboBox();
            DisplayStatistics("All Time");
        }

        private void LoadFinishedTours()
        {
            var tourRepo = new TourRepository();
            finishedTours = tourRepo.GetAll().Where(t => t.Status == TourStatus.FINISHED).ToList();
        }

        private void PopulateYearComboBox()
        {
            YearComboBox.Items.Clear();
            YearComboBox.Items.Add("All Time");
            foreach (var year in finishedTours.SelectMany(t => t.StartTimes)
                                              .Select(st => st.Time.Year)
                                              .Distinct()
                                              .OrderByDescending(y => y))
            {
                YearComboBox.Items.Add(year.ToString());
            }
            YearComboBox.SelectedIndex = 0;
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = YearComboBox.SelectedItem.ToString();
            DisplayStatistics(selected);
        }

        private void DisplayStatistics(string yearSelection)
        {
            MostVisitedPanel.Children.Clear();
            OtherToursPanel.Items.Clear();

            IEnumerable<Tour> filtered = finishedTours;
            if (yearSelection != "All Time")
            {
                int year = int.Parse(yearSelection);
                filtered = filtered.Where(t => t.StartTimes.Any(st => st.Time.Year == year));
            }

            if (!filtered.Any()) return;

            var mostVisited = filtered.OrderByDescending(t => t.ReservedSpots).FirstOrDefault();
            DisplayTourCard(mostVisited, MostVisitedPanel, true);

            foreach (var tour in filtered.Where(t => t.Id != mostVisited.Id))
            {
                OtherToursPanel.Items.Add(CreateTourCard(tour));
            }
        }

        private void DisplayTourCard(Tour tour, Panel container, bool isMostVisited)
        {
            container.Children.Add(CreateTourCard(tour));
        }

        private Border CreateTourCard(Tour tour)
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 5, 0, 5),
                Padding = new Thickness(10)
            };

            var stack = new StackPanel { Orientation = Orientation.Horizontal };

            var img = new Image
            {
                Width = 60,
                Height = 60,
                Margin = new Thickness(0, 0, 10, 0),
                Source = tour.Images?.FirstOrDefault() != null
                    ? new BitmapImage(new Uri(tour.Images.First().Path, UriKind.RelativeOrAbsolute))
                    : null
            };

            var info = new StackPanel();
            var time = tour.StartTimes.FirstOrDefault()?.Time.ToString("dd.MM.yyyy, HH:mm");
            info.Children.Add(new TextBlock { Text = tour.Name, FontWeight = FontWeights.Bold });
            info.Children.Add(new TextBlock { Text = time });
            info.Children.Add(new TextBlock { Text = tour.Language });
            info.Children.Add(new TextBlock { Text = $"Duration: {tour.DurationHours}h" });

            var detailsButton = new Button
            {
                Content = "Details",
                Margin = new Thickness(20, 0, 0, 0)
            };
            detailsButton.Click += (s, e) => ShowTourDetails(tour);

            stack.Children.Add(img);
            stack.Children.Add(info);
            stack.Children.Add(detailsButton);
            border.Child = stack;

            return border;
        }

        private void ShowTourDetails(Tour tour)
        {
            this.NavigationService.Navigate(new TourDetailsPage(tour));
        }
    }
}*/
