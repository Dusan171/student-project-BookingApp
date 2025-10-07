using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Guide;
using BookingApp.Domain.Model;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ComplexTourPartsPage : Page
    {
        public ComplexTourPartsPage(ComplexTourRequestViewModel complexTour)
        {
            InitializeComponent();
            var vm = new ComplexTourPartsViewModel(complexTour);
            DataContext = vm;

            vm.NavigateToPartDetails = (part) =>
            {
                NavigationService?.Navigate(new ComplexTourPartDetailsPage(part));
            };
        }
    }
}
