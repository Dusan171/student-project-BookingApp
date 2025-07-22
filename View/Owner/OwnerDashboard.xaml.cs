using BookingApp.Utilities;
using BookingApp.View.Owner;
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
using System.Windows.Shapes;

namespace BookingApp.View.Owner
{
    /// <summary>
    /// Interaction logic for OwnerDashboard.xaml
    /// </summary>
    public partial class OwnerDashboard : Window
    {
        public OwnerDashboard()
        {
            InitializeComponent();
            MainContentControl.Content = new HomeView(Session.CurrentUser.Username);
        }

        public void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var signInWindow = new SignInForm();
            signInWindow.Show();

            //Zatvara trenutni prozor
            this.Close();
        }
        private void HomeView_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new HomeView(Session.CurrentUser.Username);
        }

        private void RegisterAccommodation_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new RegisterAccommodationView();
        }

        private void Requests_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void Reviews_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new ReviewsView();
        }

    }
}
