using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ReviewsControl : Page
    {
        public ReviewsControl()
        {
            InitializeComponent();
            DataContext = new ReviewsControlViewModel();
        }
    }
}
