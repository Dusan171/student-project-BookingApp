using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.View.Guide;

namespace BookingApp.Presentation.View.Guide
{
    /// <summary>
    /// Interaction logic for TourStatisticsControl.xaml
    /// </summary>
    public partial class TourStatisticsControl : UserControl
    {
        private List<Tour> _finishedTours;
        private TourRepository _tourRepository;
        private TouristAttendanceRepository _attendanceRepository;
        private Tour _mostVisitedTour;
        private MainWindow mainPage;

        public TourStatisticsControl(MainWindow main)
        {
            InitializeComponent();

            _tourRepository = new TourRepository();
            _attendanceRepository = new TouristAttendanceRepository();
            mainPage = main;
            LoadFinishedTours();
            PopulateYearComboBox();
            DisplayStatistics("All Time");
        }
        private void LoadFinishedTours()
        {
            var tourRepo = new TourRepository();
            _finishedTours = tourRepo.GetAll().Where(t => t.Status == TourStatus.FINISHED).ToList();
            FillTourDetails(_finishedTours);
        }

        private void PopulateYearComboBox()
        {
            YearComboBox.Items.Clear();
            YearComboBox.Items.Add("All Time");
            foreach (var year in _finishedTours.SelectMany(t => t.StartTimes)
                                              .Select(st => st.Time.Year)
                                              .Distinct()
                                              .OrderByDescending(y => y))
            {
                YearComboBox.Items.Add(year.ToString());
            }
            YearComboBox.SelectedIndex = 0;
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
        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = YearComboBox.SelectedItem.ToString();
            DisplayStatistics(selected);
        }

        private void DisplayStatistics(string yearSelection)
        {
            MostVisitedPanel.Children.Clear();
            OtherToursPanel.Items.Clear();

            IEnumerable<Tour> filtered = _finishedTours;
            if (yearSelection != "All Time")
            {
                int year = int.Parse(yearSelection);
                filtered = filtered.Where(t => t.StartTimes.Any(st => st.Time.Year == year));
            }

            if (!filtered.Any()) return;

            var attendaces = _attendanceRepository.GetAll();
            Dictionary<int, int> tura_count = new Dictionary<int, int>();
            foreach (var tour in filtered)
            {
                tura_count.Add(tour.Id, attendaces.Where(a => a.TourId == tour.Id).Count());
            }
            var mostVisitedTourId = tura_count.OrderByDescending(x => x.Value)
                                  .Select(x => x.Key)
                                  .FirstOrDefault();
            var mostVisited = filtered.FirstOrDefault(t => t.Id == mostVisitedTourId);

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

            BitmapImage bitmap = null;
            var firstImage = tour.Images?.FirstOrDefault();
            if (firstImage != null && !string.IsNullOrWhiteSpace(firstImage.Path))
            {
                bitmap = new BitmapImage(new Uri(firstImage.Path, UriKind.RelativeOrAbsolute));
            }

            var img = new Image
            {
                Width = 60,
                Height = 60,
                Margin = new Thickness(0, 0, 10, 0),
                Source = bitmap
            };

            var info = new StackPanel();
           
            var time = tour.StartTimes.FirstOrDefault()?.Time.ToString("dd.MM.yyyy, HH:mm");
            info.Children.Add(new TextBlock { Text = tour.Name, FontWeight = FontWeights.Bold });
            info.Children.Add(new TextBlock { Text = time.ToString() });
            info.Children.Add(new TextBlock { Text = tour.Language });
            info.Children.Add(new TextBlock { Text = $"Duration: {tour.DurationHours}h" });

            var detailsButton = new Button
            {
                Content = "Details",
                Margin = new Thickness(10, 0, 0, 0)
            };

            var detailsPage = new TourDetailsControl(tour, mainPage);
            detailsButton.Click += (s, e) => mainPage.ContentFrame.Navigate(detailsPage);
            stack.Children.Add(img);
            stack.Children.Add(info);
            stack.Children.Add(detailsButton);
            border.Child = stack;

            return border;
        }
    }
}
