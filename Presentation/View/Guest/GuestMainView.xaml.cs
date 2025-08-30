using System.Windows;
using BookingApp.Presentation.ViewModel.Guest;

namespace BookingApp.Presentation.View.Guest
{
    public partial class GuestMainView : Window
    {
        public GuestMainView()
        {
            InitializeComponent();

            DataContext = new GuestMainViewModel();
        }
    }
}