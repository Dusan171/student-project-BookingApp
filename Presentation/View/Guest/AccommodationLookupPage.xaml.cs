using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Guest
{
    public partial class AccommodationLookupPage : UserControl
    {
        private AccommodationLookupViewModel _viewModel;

        public AccommodationLookupPage()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
            this.Unloaded += OnUnloaded;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.ReserveRequested -= OnReserveRequested;
            }
            _viewModel = this.DataContext as AccommodationLookupViewModel;
            if (_viewModel != null)
            {
                _viewModel.ReserveRequested += OnReserveRequested;
            }
        }
        private void OnReserveRequested(AccommodationDetailsDTO accommodationDetails)
        {
            var reservationWindow = new AccommodationReservationView(accommodationDetails);

            reservationWindow.Owner = Window.GetWindow(this);

            reservationWindow.ShowDialog();
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.ReserveRequested -= OnReserveRequested;
            }
        }
    }
}