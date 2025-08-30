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
        public string CurrentUserName => Session.CurrentUser?.FirstName + " " + Session.CurrentUser?.LastName ?? "Tourist";

        public TouristDashboardWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeViews();
            SetupEventHandlers();
            ShowSearchToursView();
        }

        private void InitializeViews()
        {
            var currentUserId = Session.CurrentUser?.Id ?? 0;

            TourPresenceContent?.InitializeViewModel(currentUserId);
            TourRequestsContent?.InitializeViewModel(currentUserId);
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
            HideAllViews();
            TourPresenceContent.Visibility = Visibility.Visible;
            SetActiveTab(TourPresenceTab);
        }

        private void TourRequests_Click(object sender, RoutedEventArgs e)
        {
            HideAllViews();
            TourRequestsContent.Visibility = Visibility.Visible;
            SetActiveTab(TourRequestsTab);
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

                    TourReviewViewContent.ViewModel.ReviewSubmitted -= OnReviewSubmitted; // ukloni stari
                    TourReviewViewContent.ViewModel.ReviewSubmitted += OnReviewSubmitted; // dodaj novi

                    TourReviewViewContent.ViewModel.LoadCompletedTours();
                }
            }
        }



        private void HideAllViews()
        {
            TourSearchView.Visibility = Visibility.Collapsed;
            TourReservationView.Visibility = Visibility.Collapsed;
            MyReservationsContent.Visibility = Visibility.Collapsed;
            TourReviewViewContent.Visibility = Visibility.Collapsed;
            ReviewsContent.Visibility = Visibility.Collapsed;
            TourPresenceContent.Visibility = Visibility.Collapsed;
            TourRequestsContent.Visibility = Visibility.Collapsed;

            TourReservationView?.ClearForm();
        }

        private void HideAllContent()
        {
            HideAllViews();
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