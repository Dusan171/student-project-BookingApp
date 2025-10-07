using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guide;
using BookingApp.Domain.Model;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ComplexTourPartDetailsPage : Page
    {
        public ComplexTourPartDetailsPage(ComplexTourRequestPart part)
        {
            InitializeComponent();
            var vm = new ComplexTourPartDetailsViewModel(part);
            vm.TourAccepted += (acceptedTour) =>
            {
                //parentVM.Requests.Remove(acceptedTour);

                if (NavigationService?.CanGoBack == true)
                    NavigationService.GoBack();
            };

            DataContext = vm;

        }
    }
}
