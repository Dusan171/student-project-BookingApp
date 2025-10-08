using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guide;
using BookingApp.Domain.Model;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ComplexTourPartsPage : Page
    {
        private ComplexTourPartsViewModel _viewModel;

        public ComplexTourPartsPage(ComplexTourRequestViewModel complexTour)
        {
            InitializeComponent();
            _viewModel = new ComplexTourPartsViewModel(complexTour);
            DataContext = _viewModel;

            _viewModel.NavigateToPartDetails = (part) =>
            {
                var detailsPage = new ComplexTourPartDetailsPage(part);
                
                // Subscribe to the PartUpdated event from the details page
                if (detailsPage.DataContext is ComplexTourPartDetailsViewModel detailsViewModel)
                {
                    detailsViewModel.PartUpdated += OnPartUpdated;
                    detailsViewModel.TourAccepted += OnTourAccepted;
                }

                NavigationService?.Navigate(detailsPage);
            };
        }

        private void OnPartUpdated(ComplexTourRequestPart updatedPart)
        {
            // Update the specific part in the collection
            _viewModel?.UpdatePart(updatedPart);
        }

        private void OnTourAccepted(ComplexTourRequest acceptedTour)
        {
            // Refresh all parts to ensure consistency
            _viewModel?.RefreshParts();
        }
    }
}
