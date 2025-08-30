using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourReservationView : UserControl
    {
        public TourReservationViewModel ViewModel { get; private set; }

        public event System.Action ReservationCompleted;
        public event System.Action ReservationCancelled;

        public TourReservationView()
        {
            InitializeComponent();
        }

        public void SetTour(TourDTO tourDto)
        {
            if (tourDto == null) return;

            ViewModel = new TourReservationViewModel(tourDto);
            DataContext = ViewModel;
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            if (ViewModel != null)
            {
                ViewModel.ReservationCompleted += OnReservationCompleted;
                ViewModel.ReservationFailed += OnReservationFailed;
            }
        }

        private void OnReservationCompleted()
        {
            MessageBox.Show($"Tura '{ViewModel?.TourName}' je uspešno rezervisana!",
                "Rezervacija uspešna", MessageBoxButton.OK, MessageBoxImage.Information);

            ReservationCompleted?.Invoke();
        }

        private void OnReservationFailed(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Greška pri rezervaciji",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationCancelled?.Invoke();
        }

        public void ClearForm()
        {
            if (ViewModel != null)
            {
                ViewModel.ReservationCompleted -= OnReservationCompleted;
                ViewModel.ReservationFailed -= OnReservationFailed;
                ViewModel.Dispose();
                ViewModel = null;
            }
            DataContext = null;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}