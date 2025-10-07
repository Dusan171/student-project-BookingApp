using System;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Utilities;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourPresenceView : UserControl
    {
        public TourPresenceViewModel? ViewModel => DataContext as TourPresenceViewModel;

        public TourPresenceView()
        {
            InitializeComponent();
        }

        
        public void InitializeViewModel(int userId)
        {
            if (DataContext is TourPresenceViewModel) return;

         
            DataContext = new TourPresenceViewModel(userId);
        }

        public void RefreshData()
        {
            try
            {
                var vm = ViewModel ?? (DataContext as TourPresenceViewModel);
                if (vm == null)
                {
                    var userId = Session.CurrentUser?.Id ?? 0;
                    vm = new TourPresenceViewModel(userId);
                    DataContext = vm;
                }
                vm.RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri osvežavanju podataka: {ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void MarkAsReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                ViewModel?.MarkNotificationAsRead(id);
            }
        }
    }
}
