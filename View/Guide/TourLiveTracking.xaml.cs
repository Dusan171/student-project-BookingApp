using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using BookingApp.Domain;
using BookingApp.Utilities;
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
            TourDateTimeTextBlock.Text = $"Date & Time: {startTime:yy-mm-dd, HH:mm:ss}";
            TourLanguageTextBlock.Text = $"Language: {currentTour.Language}";
            TourLocationTextBlock.Text = $"Location: {currentTour.Location.City}, {currentTour.Location.Country}";
            TourDurationTextBlock.Text = $"Duration: {currentTour.DurationHours}h";
        }

        private void LoadReservations()
        {
            var reservationRepository = new TourReservationRepository();
            reservations = reservationRepository.GetAll()
                          .Where(r => r.TourId == currentTour.Id && r.Status==0)
                          .ToList();

            attendanceList = reservations
                .SelectMany(r => r.Guests)
                .Select(g => new TouristAttendance
                {
                    GuestId = g.Id,
                    HasAppeared = false,
                    KeyPointJoinedAt = -1
                })
                .ToList();
        }



        private void LoadKeyPoints()
        {
            KeyPointsPanel.Children.Clear();

            bool firstChecked = true;
            int count = 1;
            foreach (var kp in currentTour.KeyPoints)
            {
                var cb = new CheckBox
                {
                    Content = $"#{count} {kp.Name}",
                    Margin = new Thickness(0, 5, 0, 5),
                    IsChecked = firstChecked
                };

                cb.Tag = kp; 
                cb.Click += KeyPointCheckBox_Click;

                firstChecked = false;
                KeyPointsPanel.Children.Add(cb);
                count++;
            }
        }
        private ReservationGuest FindGuestById(int guestId)
        {
            foreach (var reservation in reservations)
            {
                var guest = reservation.Guests.FirstOrDefault(g => g.Id == guestId);
                if (guest != null)
                    return guest;
            }
            return null;
        }


        private void KeyPointCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null) return;

            var keyPoint = cb.Tag as KeyPoint;
            if (keyPoint == null) return;

            var guests = reservations.SelectMany(r => r.Guests).ToList();
            var dialog = new ReservedTouristsWindow(currentTour, keyPoint, guests, attendanceList);
             

            if (dialog.ShowDialog() == true)
            {
                var updatedGuests = dialog.GetUpdatedAttendance();

                foreach (var guest in updatedGuests)
                {
                    var record = attendanceList.FirstOrDefault(a => a.GuestId == guest.GuestId && a.TourId == currentTour.Id);

                    if (record != null)
                    {
                        record.HasAppeared = guest.HasAppeared;
                        record.KeyPointJoinedAt = guest.HasAppeared ? keyPoint.Id : -1;
                    }
                    else
                    {
                        attendanceList.Add(new TouristAttendance
                        {
                            TourId = currentTour.Id,
                            GuestId = guest.GuestId,
                            HasAppeared = guest.HasAppeared,
                            KeyPointJoinedAt = guest.HasAppeared ? keyPoint.Id : -1
                        });
                    }
                }

                if (AllKeyPointsVisited())
                {
                    MessageBox.Show("Sve ključne tačke su prošle. Tura će biti završena.",
                                    "Kraj Ture",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    EndTour();
                }
            }
            else
            {
                cb.IsChecked = false;
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
            currentTour.IsFinished = true;

            tourRepository.Update(currentTour);

            MessageBox.Show("Tura je uspešno završena.", "Završeno", MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService?.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help is not implemented yet.");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var signInWindow = new SignInForm();
            signInWindow.Show();
        }
    }
}
