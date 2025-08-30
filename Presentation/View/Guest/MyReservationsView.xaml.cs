using System.Windows;
using BookingApp.Presentation.ViewModel.Guest;

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