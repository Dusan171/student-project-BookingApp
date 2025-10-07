using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class StatisticsSelectionControl : Page
    {
        private readonly StatisticsViewModel _viewModel;
        public StatisticsSelectionControl()
        {
            InitializeComponent();
            _viewModel = new StatisticsViewModel();
            DataContext = _viewModel;

            _viewModel.TourDetailsRequested += NavigateToTourDetails;
            _viewModel.CreateTourRequested += OpenCreateTourForm;
        }

        private void NavigateToTourDetails(Tour tour)
        {
            NavigationService?.Navigate(new TourDetailsControl(tour));
        }
        private void OpenCreateTourForm(Tour suggestedTour)
        {
            NavigationService?.Navigate(new CreateTourControl(suggestedTour));
        }
    }
}
