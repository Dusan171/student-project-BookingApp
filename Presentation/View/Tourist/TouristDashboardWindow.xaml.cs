using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.View;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;
using LibVLCSharp.Shared;
using System.Windows.Media;
using VlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TouristDashboardWindow : Window
    {
        private bool _tourPresenceInitialized = false;
        private bool _tourRequestsInitialized = false;
        private bool _tourStatisticsInitialized = false;
        private bool _tourNotificationsInitialized = false;
        private bool _complexTourRequestsInitialized = false;
        private LibVLC _libVLC;
        private VlcMediaPlayer _mediaPlayer;
        private Media _currentMedia;


        public string CurrentUserName =>
            $"{Session.CurrentUser?.FirstName} {Session.CurrentUser?.LastName}".Trim() == ""
                ? "Tourist"
                : $"{Session.CurrentUser?.FirstName} {Session.CurrentUser?.LastName}";

        public TouristDashboardWindow()
        {
            Core.Initialize();
            InitializeComponent();

            var libvlcPath = @"C:\Users\PC\.nuget\packages\videolan.libvlc.windows\3.0.21\runtimes\win-x64\native";
            Core.Initialize();
            _libVLC = new LibVLC("--plugin-path=" + System.IO.Path.Combine(libvlcPath, "plugins"), libvlcPath);
            _mediaPlayer = new VlcMediaPlayer(_libVLC);
            TutorialVideo.MediaPlayer = _mediaPlayer;

           
            DataContext = this;
            SetupEventHandlers();
            ShowSearchToursView();
        }

        private void EnsureTourPresenceInitialized()
        {
            if (!_tourPresenceInitialized && TourPresenceContent != null)
            {
                var currentUserId = Session.CurrentUser?.Id ?? 0;
                try
                {
                    TourPresenceContent.InitializeViewModel(currentUserId);
                    _tourPresenceInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri inicijalizaciji praćenja tura: {ex.Message}",
                                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EnsureTourRequestsInitialized()
        {
            if (!_tourRequestsInitialized && TourRequestsContent != null)
            {
                var currentUserId = Session.CurrentUser?.Id ?? 0;
                try
                {
                    TourRequestsContent.InitializeViewModel(currentUserId);
                    _tourRequestsInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri inicijalizaciji zahteva za ture: {ex.Message}",
                                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EnsureComplexTourRequestsInitialized()
        {
            if (!_complexTourRequestsInitialized && ComplexTourRequestsContent != null)
            {
                var currentUserId = Session.CurrentUser?.Id ?? 0;
                try
                {
                    ComplexTourRequestsContent.InitializeViewModel(currentUserId);
                    _complexTourRequestsInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri inicijalizaciji složenih zahteva: {ex.Message}",
                                    "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SetupEventHandlers()
        {
            if (TourSearchView?.ViewModel != null)
            {
                TourSearchView.ViewModel.TourReserveRequested += OnTourReserveRequested;
                TourSearchView.ViewModel.TourDetailsRequested += OnTourDetailsRequested;
            }

            if (TourReservationView != null)
            {
                TourReservationView.ReservationCompleted += OnReservationCompleted;
                TourReservationView.ReservationCancelled += OnReservationCancelled;
            }

            if (TourDetailView != null)
            {
                TourDetailView.BackRequested += OnTourDetailBackRequested;
                TourDetailView.TourReserveRequested += OnTourReserveRequested;
            }

            if (MyReservationsContent?.ViewModel != null)
            {
                MyReservationsContent.ViewModel.ReviewRequested += OnReviewRequested;
            }

            if (TourReviewViewContent?.ViewModel != null)
            {
                TourReviewViewContent.ViewModel.ReviewSubmitted += OnReviewSubmitted;
            }

            this.Closing += TouristDashboardWindow_Closing;
        }

        private void OnTourReserveRequested(TourDTO tour) => ShowReservationView(tour);

        private void OnTourDetailsRequested(TourDTO tour)
        {
            try
            {
                ShowTourDetailView(tour);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri prikazivanju detalja ture: {ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnTourDetailBackRequested()
        {
            ShowSearchToursView();
            SetActiveTab(SearchToursTab);
        }

        private void OnReservationCompleted()
        {
            ShowSearchToursView();
            SetActiveTab(SearchToursTab);
            TourSearchView?.RefreshData();
        }

        private void OnReservationCancelled()
        {
            ShowSearchToursView();
            SetActiveTab(SearchToursTab);
        }

        private void OnReviewRequested(object sender, TourReservationDTO reservation)
        {
            ShowReviewsView();
            SetActiveTab(ReviewsTab);

            TourReviewViewContent?.ViewModel?.LoadCompletedTours();
            TourReviewViewContent?.ViewModel?.SelectReservationForReview(reservation);
        }

        private void OnReviewSubmitted(object sender, int reviewedReservationId)
        {
            MyReservationsContent?.ViewModel?.RefreshCommand?.Execute(null);
        }

        
        private void SearchTours_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchToursView();
            SetActiveTab(SearchToursTab);
        }

        private void MyReservations_Click(object sender, RoutedEventArgs e)
        {
            ShowMyReservationsView();
            SetActiveTab(MyReservationsTab);
        }

        private void Reviews_Click(object sender, RoutedEventArgs e)
        {
            ShowReviewsView();
            SetActiveTab(ReviewsTab);
        }

        private void TourPresence_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TourPresenceContent == null)
                {
                    MessageBox.Show("TourPresenceContent nije inicijalizovan!", "Greška",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

              
                EnsureTourPresenceInitialized();

               
                HideAllViews();
                TourPresenceContent.Visibility = Visibility.Visible;
                SetActiveTab(TourPresenceTab);

                
                var vm = TourPresenceContent.ViewModel ?? (TourPresenceContent.DataContext as TourPresenceViewModel);
                if (vm != null) vm.RefreshData();
                else TourPresenceContent.RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju praćenja tura: {ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TourRequests_Click(object sender, RoutedEventArgs e)
        {
            EnsureTourRequestsInitialized();
            HideAllViews();
            TourRequestsContent.Visibility = Visibility.Visible;
            SetActiveTab(TourRequestsTab);
        }

        private void ComplexTourRequests_Click(object sender, RoutedEventArgs e)
        {
            EnsureComplexTourRequestsInitialized();
            HideAllViews();
            ComplexTourRequestsContent.Visibility = Visibility.Visible;
            SetActiveTab(ComplexTourRequestsTab);
        }

        private void Tutorial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideAllViews();
                TutorialContent.Visibility = Visibility.Visible;
                ResetAllTabs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju tutorijala: {ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PlayTutorial_Click(object sender, RoutedEventArgs e)
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Videos", "tutorijal.mp4");

            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show($"Video fajl nije pronađen: {path}");
                return;
            }

            _currentMedia = new Media(_libVLC, new Uri(path));
            _mediaPlayer.Play(_currentMedia);
        }

        private void PauseTutorial_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Pause();
        }

        private void StopTutorial_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
        }
        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            if (!_tourStatisticsInitialized && TourStatisticsContent != null)
            {
                var user = Session.CurrentUser;
                string fullName = user != null ? $"{user.FirstName} {user.LastName}" : "N/A";
                TourStatisticsContent.InitializeViewModel(user?.Id ?? 0, fullName);
                _tourStatisticsInitialized = true;
            }

            HideAllViews();
            TourStatisticsContent.Visibility = Visibility.Visible;
            SetActiveTab(StatisticsTab);
        }

        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
            if (!_tourNotificationsInitialized && TourNotificationsContent != null)
            {
                TourNotificationsContent.InitializeViewModel(Session.CurrentUser?.Id ?? 0);
                _tourNotificationsInitialized = true;
            }

            // Pretplatite se na oba eventa
            if (TourNotificationsContent?.ViewModel != null)
            {
                TourNotificationsContent.ViewModel.ReservationRequested -= OnNotificationReservationRequested;
                TourNotificationsContent.ViewModel.ReservationRequested += OnNotificationReservationRequested;

                // DODAJ OVO - za prikaz detalja
                TourNotificationsContent.ViewModel.ShowDetailsRequested -= OnNotificationShowDetailsRequested;
                TourNotificationsContent.ViewModel.ShowDetailsRequested += OnNotificationShowDetailsRequested;
            }

            HideAllViews();
            TourNotificationsContent.Visibility = Visibility.Visible;
            SetActiveTab(NotificationsTab);
        }

        private void OnNotificationReservationRequested(object sender, NotifiedTourDTO notifiedTour)
        {
            try
            {
                if (notifiedTour == null)
                {
                    MessageBox.Show("Greška: Podaci o turi nisu dostupni.", "Greška",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                var tourService = Injector.CreateInstance<ITourService>();
                var fullTour = tourService.GetTourById(notifiedTour.Id);

                if (fullTour != null)
                {
                    
                    var tourDTO = TourDTO.FromDomain(fullTour);
                    ShowReservationView(tourDTO);
                    SetActiveTab(SearchToursTab);
                }
                else
                {
                    MessageBox.Show("Tura nije pronađena u sistemu.", "Greška",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri otvaranju rezervacije: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da se želite odjaviti?",
                                         "Potvrda odjave",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Session.ClearSession();
                var signInWindow = new SignInForm();
                signInWindow.Show();
                this.Close();
            }
        }

        
        private void ShowSearchToursView()
        {
            HideAllViews();
            TourSearchView.Visibility = Visibility.Visible;
        }

        private void ShowReservationView(TourDTO tour)
        {
            HideAllViews();
            TourReservationView?.SetTour(tour);
            TourReservationView.Visibility = Visibility.Visible;
        }

        private void ShowTourDetailView(TourDTO tour)
        {
            if (tour == null)
            {
                MessageBox.Show("Greška: Tura nije validna", "Greška",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            HideAllViews();
            TourDetailView?.SetTour(tour);
            TourDetailView.Visibility = Visibility.Visible;
            ResetAllTabs();
        }

        private void ShowMyReservationsView()
        {
            HideAllViews();
            MyReservationsContent.Visibility = Visibility.Visible;
        }

        private void ShowReviewsView()
        {
            HideAllViews();
            TourReviewViewContent.Visibility = Visibility.Visible;

            if (TourReviewViewContent.ViewModel != null)
            {
                TourReviewViewContent.ViewModel.ReviewSubmitted -= OnReviewSubmitted;
                TourReviewViewContent.ViewModel.ReviewSubmitted += OnReviewSubmitted;
                TourReviewViewContent.ViewModel.LoadCompletedTours();
            }
        }

        private void HideAllViews()
        {
            TourSearchView.Visibility = Visibility.Collapsed;
            TourReservationView.Visibility = Visibility.Collapsed;
            TourDetailView.Visibility = Visibility.Collapsed;
            MyReservationsContent.Visibility = Visibility.Collapsed;
            TourReviewViewContent.Visibility = Visibility.Collapsed;
            TourPresenceContent.Visibility = Visibility.Collapsed;
            TourRequestsContent.Visibility = Visibility.Collapsed;
            ComplexTourRequestsContent.Visibility = Visibility.Collapsed;
            TutorialContent.Visibility = Visibility.Collapsed;
            TourStatisticsContent.Visibility = Visibility.Collapsed;
            TourNotificationsContent.Visibility = Visibility.Collapsed;

            TourReservationView?.ClearForm();
        }

        private void SetActiveTab(Button activeTab)
        {
            ResetAllTabs();
            activeTab.IsDefault = true;
        }

        private void ResetAllTabs()
        {
            SearchToursTab.IsDefault = false;
            MyReservationsTab.IsDefault = false;
            ReviewsTab.IsDefault = false;
            TourPresenceTab.IsDefault = false;
            TourRequestsTab.IsDefault = false;
            ComplexTourRequestsTab.IsDefault = false;
            StatisticsTab.IsDefault = false;
            NotificationsTab.IsDefault = false;
        }

        private void TouristDashboardWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (TourSearchView?.ViewModel != null)
            {
                TourSearchView.ViewModel.TourReserveRequested -= OnTourReserveRequested;
                TourSearchView.ViewModel.TourDetailsRequested -= OnTourDetailsRequested;
            }

            if (TourReservationView != null)
            {
                TourReservationView.ReservationCompleted -= OnReservationCompleted;
                TourReservationView.ReservationCancelled -= OnReservationCancelled;
            }

            if (TourDetailView != null)
            {
                TourDetailView.BackRequested -= OnTourDetailBackRequested;
                TourDetailView.TourReserveRequested -= OnTourReserveRequested;
                TourDetailView.Cleanup();
            }

            if (MyReservationsContent?.ViewModel != null)
            {
                MyReservationsContent.ViewModel.ReviewRequested -= OnReviewRequested;
            }

            if (TourReviewViewContent?.ViewModel != null)
            {
                TourReviewViewContent.ViewModel.ReviewSubmitted -= OnReviewSubmitted;
            }

            if (TourNotificationsContent?.ViewModel != null)
            {
                TourNotificationsContent.ViewModel.ReservationRequested -= OnNotificationReservationRequested;
            }

            if (TourNotificationsContent?.ViewModel != null)
            {
                TourNotificationsContent.ViewModel.ReservationRequested -= OnNotificationReservationRequested;
                TourNotificationsContent.ViewModel.ShowDetailsRequested -= OnNotificationShowDetailsRequested; // DODAJ
            }
        }

        private void OnNotificationShowDetailsRequested(object sender, TourNotificationDTO notification)
        {
            try
            {
                if (notification?.Tour == null)
                {
                    MessageBox.Show("Nema dostupnih detalja za ovu turu.",
                                  "Greška",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                
                var tourService = Injector.CreateInstance<ITourService>();
                var fullTour = tourService.GetTourById(notification.Tour.Id);

                if (fullTour != null)
                {
                    
                    var tourDTO = TourDTO.FromDomain(fullTour);

                    
                    ShowSearchToursView();
                    SetActiveTab(SearchToursTab);

                    
                    OnTourDetailsRequested(tourDTO);
                }
                else
                {
                    MessageBox.Show("Tura nije pronađena u sistemu.",
                                  "Greška",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri prikazivanju detalja: {ex.Message}",
                               "Greška",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }
        

        public void RefreshCurrentView()
        {
            if (TourSearchView?.Visibility == Visibility.Visible)
            {
                TourSearchView.RefreshData();
            }
        }
    }
}
