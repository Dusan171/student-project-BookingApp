using System.Windows.Controls;
using BookingApp.Domain.Model;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class TourRequestDetailsPage : Page
    {
        public TourRequestDetailsPage(TourRequest request, TourRequestsViewModel parentVM)
        {
            InitializeComponent();

            var vm = new TourRequestDetailsViewModel(request);
            vm.TourAccepted += (acceptedTour) =>
            {
                parentVM.Requests.Remove(acceptedTour);

                if (NavigationService?.CanGoBack == true)
                    NavigationService.GoBack();
            };

            DataContext = vm;
        }
    }

}
