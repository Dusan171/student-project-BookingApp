using System.Windows.Controls;
using BookingApp.Domain.Model;
using System.Collections.Generic;
using BookingApp.Repositories;
using BookingApp.Presentation.ViewModel.Guide;
using System.Windows;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ReservedTouristsPage : Page
    {
        public ReservedTouristsPage(Tour tour, List<KeyPoint> passedKeyPoints, List<ReservationGuest> guests, List<TouristAttendance> attendance)
        {
            InitializeComponent();

            var vm = new ReservedTouristsViewModel();
            vm.Initialize(tour, passedKeyPoints, guests, attendance);
            DataContext = vm;

            vm.PageClosed += OnPageClosed;
        }

        private void OnPageClosed(object? sender, bool saved)
        {
            if (saved)
            {
                MessageBox.Show("Promene su sačuvane!");
            }
            else
            {
                MessageBox.Show("Otkazano.");
            }

            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }


}
