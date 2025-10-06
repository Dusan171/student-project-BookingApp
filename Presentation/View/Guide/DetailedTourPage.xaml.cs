using BookingApp.Domain.Model;
using System.Windows.Controls;

namespace BookingApp.Presentation.View.Guide
{
    public partial class DetailedTourPage : Page
    {
        public DetailedTourPage(Tour tour)
        {
            InitializeComponent();
            DataContext = new DetailedTourViewModel(tour);
        }
    }
}

