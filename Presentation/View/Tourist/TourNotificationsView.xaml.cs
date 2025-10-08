using System;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourNotificationsView : UserControl
    {
        public TourNotificationsViewModel ViewModel { get; private set; }

        public TourNotificationsView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(int userId)
        {
            try
            {
                ViewModel = new TourNotificationsViewModel(userId);
                DataContext = ViewModel;

              
            }
            catch (Exception ex)
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusMessage = $"Greška pri inicijalizaciji obaveštenja: {ex.Message}";
                }
            }
        }

        public void RefreshNotifications()
        {
            ViewModel?.LoadNotifications();
        }

        
    }
}