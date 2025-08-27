using BookingApp.Domain.Model;
using BookingApp.Presentation.ViewModel.Owner;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;

namespace BookingApp.Presentation.View.Owner
{
    public partial class Notifications : UserControl
    {
        public Notifications()
        {
            InitializeComponent();
            var viewModel = new NotificationsViewModel();
            viewModel.RateGuestRequested += ShowGuestRatingForm;
            DataContext = viewModel;
        }

        public void LoadNotifications(List<Notification> notifications)
        {
            if (DataContext is NotificationsViewModel viewModel)
            {
                viewModel.AddNotifications(notifications);
            }
        }

        private void ShowGuestRatingForm(int reservationId, int guestId)
        {
            var ratingWindow = new GuestRatingForm(reservationId, guestId);
            ratingWindow.ShowDialog();
        }
    }
}