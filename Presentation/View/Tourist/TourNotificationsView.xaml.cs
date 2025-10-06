using System;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourNotificationsView : UserControl
    {
        public TourNotificationsViewModel ViewModel { get; private set; }
        public event EventHandler<NotifiedTourDTO> ReservationRequested;

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

                ViewModel.ReservationRequested += OnReservationRequested;
            }
            catch (Exception ex)
            {
               
                if (ViewModel != null)
                {
                    ViewModel.StatusMessage = $"Greška pri inicijalizaciji obaveštenja: {ex.Message}";
                }
            }
        }

        private void OnReservationRequested(object sender, NotifiedTourDTO tour)
        {
            try
            {
                ReservationRequested?.Invoke(this, tour);
            }
            catch (Exception ex)
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusMessage = $"Greška pri rezervaciji: {ex.Message}";
                }
            }
        }

        public void RefreshNotifications()
        {
            try
            {
                
                ViewModel?.LoadNotifications();
            }
            catch (Exception ex)
            {
                if (ViewModel != null)
                {
                    ViewModel.StatusMessage = $"Greška pri osvežavanju: {ex.Message}";
                }
            }
        }
    }
}