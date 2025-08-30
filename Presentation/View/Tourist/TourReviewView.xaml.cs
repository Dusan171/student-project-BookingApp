using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourReviewView : UserControl
    {
        public TourReviewView()
        {
            InitializeComponent();
            DataContext = new TourReviewViewModel();
        }

        public TourReviewViewModel? ViewModel => DataContext as TourReviewViewModel;

        public void LoadCompletedTours()
        {
            ViewModel?.LoadCompletedTours();
        }
    }
}