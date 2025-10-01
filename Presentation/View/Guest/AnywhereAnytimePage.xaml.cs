using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Utilities;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AnywhereAnytimePage : UserControl
    {
        public AnywhereAnytimePage()
        {
            InitializeComponent();
            DataContext = new AnywhereAnytimeViewModel();
        }
    }
}
