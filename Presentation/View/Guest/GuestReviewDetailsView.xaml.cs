using System.Windows;
using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Guest
{
    public partial class GuestReviewDetailsView : Window
    {
        public GuestReviewDetailsView(GuestReviewDTO reviewDto)
        {
            InitializeComponent();

            DataContext = new GuestReviewDetailsViewModel(reviewDto);
        }
    }
}
