using System.Windows.Controls;
using BookingApp.Services;
using BookingApp.Repositories;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class TourRequestsPage : Page
    {
        private readonly TourRequestsViewModel _viewModel;
        private UserRepository userRepo;
        public TourRequestsPage()
        {
            InitializeComponent();
            userRepo = new UserRepository();
            _viewModel = new TourRequestsViewModel(userRepo);
            DataContext = _viewModel;
            _viewModel.NavigateToTourDetails = (request) =>
            {
                NavigationService?.Navigate(new TourRequestDetailsPage(request, _viewModel));
            };

            _viewModel.NavigateToComplexParts = (complexTour) =>
            {
                NavigationService?.Navigate(new ComplexTourPartsPage(complexTour));
            };
        }
    }
}
