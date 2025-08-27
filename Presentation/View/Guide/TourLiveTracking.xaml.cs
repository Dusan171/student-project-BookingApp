using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Domain;
using BookingApp.Repositories;
using BookingApp.Domain.Model;

namespace BookingApp.View.Guide
{
    public partial class TourLiveTracking : Page
    {
        private Tour currentTour;
        private List<TourReservation> reservations;
        private TourRepository tourRepository = new TourRepository();
        private List<TouristAttendance> attendanceList = new List<TouristAttendance>();
        private TouristAttendanceRepository attendanceRepo = new TouristAttendanceRepository();

        public TourLiveTracking(Tour tour, DateTime startTime)
        {
            InitializeComponent();

            currentTour = tour ?? throw new ArgumentNullException(nameof(tour));
            LoadReservations();
            LoadTourDetails(startTime);
            LoadKeyPoints();
        }

        private void LoadTourDetails(DateTime startTime)
        {
            TourNameTextBlock.Text = currentTour.Name;
            TourDateTimeTextBlock.Text = $"Date & Time: {startTime:yyyy-MM-dd HH:mm}";
            TourLanguageTextBlock.Text = $"Language: {currentTour.Language}";
            TourLocationTextBlock.Text = $"Location: {currentTour.Location.City}, {currentTour.Location.Country}";
            TourDurationTextBlock.Text = $"Duration: {currentTour.DurationHours}h";
        }

        private void LoadReservations()
        {
            var reservationRepository = new TourReservationRepository();
            reservations = reservationRepository.GetAll()
                          .Where(r => r.TourId == currentTour.Id && r.Status == 0)
                          .ToList();

            attendanceList = reservations
                .SelectMany(r => r.Guests)
                .Select(g => new TouristAttendance
                {
                    TourId = currentTour.Id,
                    GuestId = g.Id,
                    HasAppeared = false,
                    KeyPointJoinedAt = -1
                })
                .ToList();
        }

        private void LoadKeyPoints()
        {
            KeyPointsPanel.Children.Clear();

            int count = 1;
            foreach (var kp in currentTour.KeyPoints)
            {
                var cb = new CheckBox
                {
                    Content = $"#{count} {kp.Name}",
                    Margin = new Thickness(0, 5, 0, 5)
                };

                cb.Tag = kp;
                cb.Click += KeyPointCheckBox_Click;

                KeyPointsPanel.Children.Add(cb);
                count++;
            }

            if (KeyPointsPanel.Children.Count > 0 && KeyPointsPanel.Children[0] is CheckBox firstCb)
                firstCb.IsChecked = true;
        }

        private void KeyPointCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AllKeyPointsVisited())
            {
                OpenTouristWindow();
                MessageBox.Show("Sve ključne tačke su prošle. Tura će biti završena.",
                                "Kraj Ture",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                EndTour();
            }
        }

        private bool AllKeyPointsVisited()
        {
            foreach (var child in KeyPointsPanel.Children)
            {
                if (child is CheckBox cb && cb.IsChecked != true)
                    return false;
            }
            return true;
        }
        private void TouristsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenTouristWindow();
        }
        private void OpenTouristWindow()
        {
            var passedKeyPoints = currentTour.KeyPoints
                                    .Where(kp =>
                                        KeyPointsPanel.Children
                                        .OfType<CheckBox>()
                                        .Any(cb => cb.Tag == kp && cb.IsChecked == true))
                                    .ToList();

            if (!passedKeyPoints.Any())
            {
                MessageBox.Show("Nema označenih tačaka – turisti se ne mogu dodeliti.");
                return;
            }

            var guests = reservations.SelectMany(r => r.Guests).ToList();

            var dialog = new ReservedTouristsWindow(currentTour, passedKeyPoints, guests, attendanceList);

            if (dialog.ShowDialog() == true)
            {
                attendanceList = dialog.GetUpdatedAttendance();
                WriteAttendantsToFile(attendanceList);
            }
        }
        private void WriteAttendantsToFile(List<TouristAttendance> attendantsList)
        {
            foreach (var attendant in attendantsList)
            {
                var existing = attendanceRepo.GetAll().FirstOrDefault(a => a.Id == attendant.Id);
                if (existing == null)
                {
                    attendanceRepo.Save(attendant); 
                }
                else
                {
                    existing.GuestId = attendant.GuestId;
                    existing.TourId = attendant.TourId;
                    existing.HasAppeared = attendant.HasAppeared;
                    existing.KeyPointJoinedAt = attendant.KeyPointJoinedAt;
                    attendanceRepo.Update(existing); 
                }
            }
        }


        private void StopTourButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da želite da završite turu ranije?",
                                          "Potvrda",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                EndTour();
            }
        }

        private void EndTour()
        {
            currentTour.Status = TourStatus.FINISHED;
            tourRepository.Update(currentTour);

            MessageBox.Show("Tura je uspešno završena.", "Završeno", MessageBoxButton.OK, MessageBoxImage.Information);

            var mainPage = new MainWindow();
            this.NavigationService.Navigate(mainPage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Klikni na ključne tačke kako bi ih obeležio. Dugme 'Turisti' otvara listu turista i omogućava da označiš kada su se pridružili.");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            /*Session.CurrentUser = null;
            var signInWindow = new SignInForm();
            signInWindow.Show();*/
        }

    }
}
