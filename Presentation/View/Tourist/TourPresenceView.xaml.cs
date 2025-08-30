using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;
using BookingApp.Domain.Interfaces.ServiceInterfaces;
using BookingApp.Domain.Interfaces;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourPresenceView : UserControl
    {
        public TourPresenceView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(int userId)
        {
            var presenceService = Injector.CreateInstance<ITourPresenceService>();
            var tourService = Injector.CreateInstance<ITourService>();
            DataContext = new TourPresenceViewModel(presenceService, tourService, userId);
        }
    }
}