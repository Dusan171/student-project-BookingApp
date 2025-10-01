using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guest;

namespace BookingApp.Presentation.View.Guest
{
    public partial class MyReservationsPage : UserControl
    {
        public MyReservationsPage()
        {
            InitializeComponent();
            DataContext = new MyReservationsViewModel();
        }
    }
}