using System;
using System.ComponentModel;
using System.Windows;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Guest
{
    public partial class GuestMainWindow : Window
    {
        private readonly GuestMainViewModel _viewModel;
        public GuestMainWindow()
        {
            InitializeComponent();
            _viewModel = new GuestMainViewModel();
            this.DataContext = _viewModel;

            _viewModel.InitializeSubscribers();

            GuestMainViewModel.OpenReservationWindowRequested += OnOpenReservationWindowRequested;
            GuestMainViewModel.CloseMainWindowRequested += OnCloseMainWindowRequested;

            this.Closing += OnWindowClosing;
        }
        private void OnCloseMainWindowRequested()
        {
            this.Close();
        }
        private void OnOpenReservationWindowRequested(AccommodationDetailsDTO accommodation)
        {
            var reservationWindow = new AccommodationReservationView(accommodation);
            reservationWindow.Owner = this;
            reservationWindow.ShowDialog();
        }
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            GuestMainViewModel.OpenReservationWindowRequested -= OnOpenReservationWindowRequested;
            GuestMainViewModel.CloseMainWindowRequested -= OnCloseMainWindowRequested;

            _viewModel.Cleanup();
        }
    }
}