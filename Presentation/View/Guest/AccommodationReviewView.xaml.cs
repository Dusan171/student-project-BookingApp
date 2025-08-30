using System.Windows;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AccommodationReviewView : Window
    {
        public AccommodationReviewView(ReservationDetailsDTO reservationDetails)
        {
            InitializeComponent();

            var viewModel = new AccommodationReviewViewModel(reservationDetails);

            DataContext = viewModel;

            viewModel.CloseAction = new System.Action(() => this.DialogResult = true);
            viewModel.CloseAction += new System.Action(this.Close);
        }
    }
}
