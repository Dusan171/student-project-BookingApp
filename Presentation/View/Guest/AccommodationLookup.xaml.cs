using BookingApp.Presentation.ViewModel.Guest; 
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AccommodationLookup : Window
    {
        public AccommodationLookup()
        {
            InitializeComponent();
  
            DataContext = new AccommodationLookupViewModel();
        }
    }
}