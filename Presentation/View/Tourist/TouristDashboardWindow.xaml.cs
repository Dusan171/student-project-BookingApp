using System;
using System.ComponentModel;
using System.Windows;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.View;
using BookingApp.Presentation.ViewModel.Tourist;
using System.Windows.Controls;
using BookingApp.Domain.Interfaces;
using BookingApp.Services;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TouristDashboardWindow : Window
    {
        private bool _tourPresenceInitialized = false;
        private bool _tourRequestsInitialized = false;

        public string CurrentUserName => Session.CurrentUser?.FirstName + " " + Session.CurrentUser?.LastName ?? "Tourist";

        public TouristDashboardWindow()
        {
            InitializeComponent();
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

        private void SetupEventHandlers()
        {
            if (TourSearchView?.ViewModel != null)
            {
                TourSearchView.ViewModel.TourReserveRequested += OnTourReserveRequested;
            }

            if (TourReservationView != null)
            {
                TourReservationView.ReservationCompleted += OnReservationCompleted;
                TourReservationView.ReservationCancelled += OnReservationCancelled;
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

        private void OnTourReserveRequested(TourDTO tour)
        {
            ShowReservationView(tour);
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

            if (TourReviewViewContent?.ViewModel != null)
            {
                TourReviewViewContent.ViewModel.LoadCompletedTours();
                TourReviewViewContent.ViewModel.SelectReservationForReview(reservation);
            }
        }

        private void OnReviewSubmitted(object sender, int reviewedReservationId)
        {
            if (MyReservationsContent?.ViewModel != null)
            {
                MyReservationsContent.ViewModel.RefreshCommand?.Execute(null);
            }
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
                TourPresenceContent.RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju praćenja tura: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TourRequests_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureTourRequestsInitialized();
                HideAllViews();
                TourRequestsContent.Visibility = Visibility.Visible;
                SetActiveTab(TourRequestsTab);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju zahteva za ture: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Da li ste sigurni da se želite odjaviti?",
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
            if (TourSearchView != null)
                TourSearchView.Visibility = Visibility.Visible;
        }

        private void ShowReservationView(TourDTO tour)
        {
            HideAllViews();
            TourReservationView?.SetTour(tour);
            if (TourReservationView != null)
                TourReservationView.Visibility = Visibility.Visible;
        }

        private void ShowMyReservationsView()
        {
            HideAllViews();
            if (MyReservationsContent != null)
                MyReservationsContent.Visibility = Visibility.Visible;
        }

        private void ShowReviewsView()
        {
            HideAllViews();
            if (TourReviewViewContent != null)
            {
                TourReviewViewContent.Visibility = Visibility.Visible;

                if (TourReviewViewContent.ViewModel != null)
                {
                    TourReviewViewContent.ViewModel.ReviewSubmitted -= OnReviewSubmitted;
                    TourReviewViewContent.ViewModel.ReviewSubmitted += OnReviewSubmitted;
                    TourReviewViewContent.ViewModel.LoadCompletedTours();
                }
            }
        }

        private void HideAllViews()
        {
            if (TourSearchView != null) TourSearchView.Visibility = Visibility.Collapsed;
            if (TourReservationView != null) TourReservationView.Visibility = Visibility.Collapsed;
            if (MyReservationsContent != null) MyReservationsContent.Visibility = Visibility.Collapsed;
            if (TourReviewViewContent != null) TourReviewViewContent.Visibility = Visibility.Collapsed;
            if (ReviewsContent != null) ReviewsContent.Visibility = Visibility.Collapsed;
            if (TourPresenceContent != null) TourPresenceContent.Visibility = Visibility.Collapsed;
            if (TourRequestsContent != null) TourRequestsContent.Visibility = Visibility.Collapsed;

            TourReservationView?.ClearForm();
        }

        private void SetActiveTab(Button activeTab)
        {
            SearchToursTab.IsDefault = false;
            MyReservationsTab.IsDefault = false;
            ReviewsTab.IsDefault = false;
            TourPresenceTab.IsDefault = false;
            TourRequestsTab.IsDefault = false;

            activeTab.IsDefault = true;
        }

        private void TouristDashboardWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (TourSearchView?.ViewModel != null)
            {
                TourSearchView.ViewModel.TourReserveRequested -= OnTourReserveRequested;
            }

            if (TourReservationView != null)
            {
                TourReservationView.ReservationCompleted -= OnReservationCompleted;
                TourReservationView.ReservationCancelled -= OnReservationCancelled;
            }

            if (MyReservationsContent?.ViewModel != null)
            {
                MyReservationsContent.ViewModel.ReviewRequested -= OnReviewRequested;
            }

            if (TourReviewViewContent?.ViewModel != null)
            {
                TourReviewViewContent.ViewModel.ReviewSubmitted -= OnReviewSubmitted;
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