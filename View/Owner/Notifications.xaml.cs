using BookingApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookingApp.View.Owner
{
    /// <summary>
    /// Interaction logic for Notifications.xaml
    /// </summary>
    public partial class Notifications : UserControl
    {
        public Notifications()
        {
            InitializeComponent();

        }

        public void LoadNotifications(List<Notification> notifications)
        {
            NotificationsPanel.Children.Clear(); 

            foreach (var notification in notifications)
            {
                var border = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    Background = System.Windows.Media.Brushes.White,
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10),
                    Margin = new Thickness(5)
                };

                var stack = new StackPanel();

                var guestText = new TextBlock
                {
                    Text = $"❗ Guest ID: {notification.GuestId}",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var deadlineText = new TextBlock
                {
                    Text = $"Rating deadline: {notification.Deadline.ToShortDateString()}",
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var button = new Button
                {
                    Content = "Rate now",
                    Width = 100,
                    Tag = Tuple.Create(notification.ReservationId, notification.GuestId)
                };

                button.Click += RateButton_Click;

                stack.Children.Add(guestText);
                stack.Children.Add(deadlineText);
                stack.Children.Add(button);
                border.Child = stack;

                NotificationsPanel.Children.Add(border);
            }

            if (notifications.Count == 0)
            {
                var emptyText = new TextBlock
                {
                    Text = "✔️ All guests are rated.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10),
                    FontStyle = FontStyles.Italic,
                    Foreground = System.Windows.Media.Brushes.Gray
                };
                NotificationsPanel.Children.Add(emptyText);
            }
        }

        private void RateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tag = (Tuple<int, int>)button.Tag;

            int reservationId = tag.Item1;
            int guestId = tag.Item2;

           
            var ratingWindow = new GuestRatingForm(reservationId, guestId);
            ratingWindow.ShowDialog();

        }

    }
}
