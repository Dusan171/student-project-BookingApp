using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class TourDetailsControl : Page
    {
        public TourDetailsControl(Tour tour)
        {
            InitializeComponent();
            DataContext = new TourDetailsControlViewModel(tour);
        }
        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
        }
    }
}
