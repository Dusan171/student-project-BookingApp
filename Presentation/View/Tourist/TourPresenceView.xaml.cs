using System;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;
using BookingApp.Domain.Interfaces;
using BookingApp.Utilities;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourPresenceView : UserControl
    {
        public TourPresenceViewModel? ViewModel => DataContext as TourPresenceViewModel;

        public TourPresenceView()
        {
           
            InitializeComponent();

            
            if (DataContext == null)
            {
                
            }
        }
        public void InitializeViewModel(int userId)
        {
            

            if (DataContext != null)
            {
                return;
            }

            try
            {
                
                var presenceService = Injector.CreateInstance<ITourPresenceService>();

                var tourService = Injector.CreateInstance<ITourService>();
               
                DataContext = new TourPresenceViewModel(presenceService, tourService, userId);

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GREŠKA pri kreiranju ViewModel: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public void RefreshData()
        {
            
            try
            {
                if (ViewModel != null)
                {
                    ViewModel.LoadTourPresenceData();
                }
                else
                {
                 
                    // Pokušaj da dobije ViewModel direktno iz DataContext
                    if (DataContext is TourPresenceViewModel directViewModel)
                    {   
                        directViewModel.LoadTourPresenceData();
                    }
                    else
                    {
                        Console.WriteLine($" DataContext nije TourPresenceViewModel! Tip: {DataContext?.GetType().Name ?? "NULL"}");
                    }
                }
            }
            catch (Exception ex)
            {
               
                MessageBox.Show($"Greška pri osvežavanju podataka: {ex.Message}",
                               "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel != null)
                {
                    ViewModel.RefreshData();

                    MessageBox.Show($"Podaci su osveženi!\nAktivnih tura: {ViewModel.ActiveTourPresences?.Count ?? 0}\nNeprочitanih obaveštenja: {ViewModel.UnreadNotifications?.Count ?? 0}",
                                   "Osveženo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Greška: ViewModel nije dostupan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri osvežavanju: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void MarkAsReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int notificationId)
            {
                ViewModel?.MarkNotificationAsRead(notificationId);
            }
        }
    }
}