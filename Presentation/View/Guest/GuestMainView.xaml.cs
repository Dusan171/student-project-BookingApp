using BookingApp.Presentation.ViewModel; // Namespace gde je ViewModel
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class GuestMainView : Window
    {
        public GuestMainView()
        {
            InitializeComponent();
            // Povezujemo View sa ViewModel-om
            DataContext = new GuestMainViewModel();
        }
    }
}