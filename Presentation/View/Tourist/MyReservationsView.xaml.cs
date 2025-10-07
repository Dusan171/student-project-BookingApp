using System;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class MyReservationsView : UserControl
    {
        public MyReservationsViewModel ViewModel { get; private set; }
        public event EventHandler<TourReservationDTO>? ReviewRequested;

        public MyReservationsView()
        {
            InitializeComponent();
            ViewModel = new MyReservationsViewModel();
            DataContext = ViewModel;

            
            ViewModel.ReviewRequested += OnReviewRequested;
        }

        private void OnReviewRequested(object? sender, TourReservationDTO reservation)
        {
            
            ReviewRequested?.Invoke(this, reservation);
        }
    }
}