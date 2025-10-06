using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourRequestStatisticsView : UserControl
    {
        public TourRequestStatisticsView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(int userId, string userName)
        {
            var statisticsService = Injector.CreateInstance<BookingApp.Domain.Interfaces.ITourRequestStatisticsService>();
            var viewModel = new TourRequestStatisticsViewModel(userId, userName, statisticsService);
            DataContext = viewModel;
        }
    }
}