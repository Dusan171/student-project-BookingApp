using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Utilities;
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
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private string username;
        private ReservationRepository reservationRepository;
        private GuestReviewRepository guestReviewRepository;
        private bool HasUnratedGuests=false;
        private const string ToastInfoFile = "toastShownDate.txt";
        public HomeView(string username)
        {
            InitializeComponent();
            this.username = username;
            UsernameTextBlock.Text = username;
            reservationRepository = new ReservationRepository();
            guestReviewRepository = new GuestReviewRepository();
            MainContent.Content = new WelcomeView();
            CheckForUnratedGuests();

        }

        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
            var notificationsView = new Notifications();
            List<Reservation> allReservations = reservationRepository.GetAll();
            var notifications = NotificationsGenerator.Generate(allReservations, guestReviewRepository);

            notificationsView.LoadNotifications(notifications);
            MainContent.Content = notificationsView;
        }
    
        private void Community_Click(object sender, RoutedEventArgs e)
        {
      
        }

        private void Library_Click(object sender, RoutedEventArgs e)
        {
   
        }
        private void CheckForUnratedGuests()
        {
            List<Reservation> allReservations = reservationRepository.GetAll();
            var notifications = NotificationsGenerator.Generate(allReservations, guestReviewRepository);

            HasUnratedGuests = notifications.Any();

            if (HasUnratedGuests)
            {
                NotificationsButton.Background = Brushes.Red;
                NotificationsButton.Content = "Notifications *";
                if (!ToastShownToday())
                {
                    ShowToast("🔔 You have guests to rate!");
                    SaveToastShownDate(DateTime.Now);
                }
            }
            else
            {
                NotificationsButton.Background = Brushes.LightGray; 
                NotificationsButton.Content = "Notifications";
            }
        }
        private void ShowToast(string message)
        {
            ToastMessage.Text = message;
            ToastPopup.Visibility = Visibility.Visible;
            ToastPopup.Opacity = 1;

            var animation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                BeginTime = TimeSpan.FromSeconds(3)
            };

            animation.Completed += (s, e) =>
            {
                ToastPopup.Visibility = Visibility.Collapsed;
            };

            ToastPopup.BeginAnimation(UIElement.OpacityProperty, animation);
        }
        private bool ToastShownToday()
        {
            if (!System.IO.File.Exists(ToastInfoFile))
                return false;

            string content = System.IO.File.ReadAllText(ToastInfoFile);
            if (DateTime.TryParse(content, out DateTime lastShown))
            {
                return lastShown.Date == DateTime.Today;
            }

            return false;
        }

        private void SaveToastShownDate(DateTime date)
        {
            System.IO.File.WriteAllText(ToastInfoFile, date.ToString("yyyy-MM-dd"));
        }
    }
}
