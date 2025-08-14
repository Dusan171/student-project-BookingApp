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
using System.Collections.Generic;
using BookingApp.Domain;

namespace BookingApp.View
{
    public partial class AlternativeToursWindow : Window
    {
        private Tourist _loggedInTourist;

        public AlternativeToursWindow(List<Tour> alternativeTours, Tourist loggedInTourist)
        {
            InitializeComponent();
            _loggedInTourist = loggedInTourist;
            AlternativeToursList.ItemsSource = alternativeTours;
        }

        private void ReserveAlternative_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is Tour selectedTour)
            {
                var reservationWindow = new TourReservationWindow(selectedTour, _loggedInTourist);
                reservationWindow.ShowDialog();
                Close();
            }
        }
    }
}

