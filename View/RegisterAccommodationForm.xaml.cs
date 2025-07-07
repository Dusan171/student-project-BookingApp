using System.Windows;
using BookingApp.Utilities;

namespace BookingApp.View
{
    /// <summary>
    /// Interaction logic for RegisterAccommodationForm.xaml
    /// </summary>
    public partial class RegisterAccommodationForm : Window
    {
        public RegisterAccommodationForm()
        {
            InitializeComponent();
        }


        public void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var signInWindow = new SignInForm();
            signInWindow.Show();

            //Zatvara trenutni prozor
            this.Close();
        }



    }
}
