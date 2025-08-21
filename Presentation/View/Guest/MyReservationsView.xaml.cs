using BookingApp.Presentation.ViewModel; // Namespace gde je ViewModel
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class MyReservationsView : Window
    {
        public MyReservationsView()
        {
            InitializeComponent();
            DataContext = new MyReservationsViewModel();
        }
    }
}