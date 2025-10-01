using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;
using System.Windows;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AccommodationReservationView : Window
    {
        public AccommodationReservationView(AccommodationDetailsDTO accommodationDetails)
        {
            InitializeComponent();
            this.DataContext = new AccommodationReservationViewModel(accommodationDetails);
        }
    }
}