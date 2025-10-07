using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Guest
{
    public partial class MyReservationsPage : UserControl
    {
        private MyReservationsViewModel _viewModel;

        public MyReservationsPage()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.RescheduleRequested -= OnRescheduleRequested;
                _viewModel.RateRequested -= OnRateRequested;
            }

            _viewModel = this.DataContext as MyReservationsViewModel;

            if (_viewModel != null)
            {
                _viewModel.RescheduleRequested += OnRescheduleRequested;
                _viewModel.RateRequested += OnRateRequested;
            }
        }

        private void OnRescheduleRequested(ReservationDetailsDTO dto)
        {
            var rescheduleWindow = new RescheduleRequestView(dto);
            rescheduleWindow.ShowDialog();
            _viewModel.LoadReservations(); 
        }

        private void OnRateRequested(ReservationDetailsDTO dto)
        {
            var rateWindow = new AccommodationReviewView(dto);
            rateWindow.ShowDialog();
            _viewModel.LoadReservations(); 
        }
    }
}