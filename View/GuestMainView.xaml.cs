using System.Windows;

namespace BookingApp.View
{
    /// <summary>
    /// Interaction logic for GuestMainView.xaml
    /// </summary>
    public partial class GuestMainView : Window
    {
        public GuestMainView()
        {
            InitializeComponent();
        }
        private void ViewAccommodations_Click(object sender, RoutedEventArgs e)
        {
            var accommodationsWindow = new AccommodationLookup();
            accommodationsWindow.ShowDialog();
        }
        private void MyReservations_Click(object sender, RoutedEventArgs e)
        {
            var myReservationsWindow = new MyReservationsView();
            myReservationsWindow.ShowDialog();
        }
    }
}
