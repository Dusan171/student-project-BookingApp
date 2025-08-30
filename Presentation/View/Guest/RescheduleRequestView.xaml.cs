using System.Windows;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Guest
{
    public partial class RescheduleRequestView : Window
    {
        public RescheduleRequestView(ReservationDetailsDTO reservationDetails)
        {
            InitializeComponent();

            var viewModel = new RescheduleRequestViewModel(reservationDetails);

            DataContext = viewModel;

            viewModel.CloseAction = () => { this.DialogResult = true; this.Close(); };
        }
    }
}