using BookingApp.Presentation.ViewModel; // Namespace gde je vaš novi ViewModel
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AccommodationLookup : Window
    {
        public AccommodationLookup()
        {
            InitializeComponent();
            // Povezujemo View sa ViewModel-om
            DataContext = new AccommodationLookupViewModel();
        }
    }
}